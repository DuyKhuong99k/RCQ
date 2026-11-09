using System.Runtime.CompilerServices;
using Models.Repos.Models;
using Vars.Hubs;

namespace Services;

/// <summary>Confirmed runtime selectors only. No SQL, firmware changes, timers or global machine lock.</summary>
public static class HqScaleSelection
{
    public const string Waiting = "Đang đổi mã. Chờ xác nhận";
    public const string Unconfirmed = "Chưa xác nhận mã. Bấm Set lại";
    public const string Changed = "Mã cân vừa đổi. Quẹt lại";
    private static readonly TimeSpan ConfirmationWindow = TimeSpan.FromSeconds(10);
    private static readonly ConditionalWeakTable<MayCan, State> States = new();
    private sealed class State
    {
        public State() { }
        public long Revision;
        public readonly Dictionary<string, Pending> Pending = new(StringComparer.Ordinal);
    }
    private sealed record Pending(string Value, long Started);
    public sealed record Selection(string? Lot, string? Size, string? Product, string? Material,
        long Revision, string? Error);

    public static bool IsSelector(string property) => property is nameof(MayCan.MaLo) or nameof(MayCan.MaSize)
        or nameof(MayCan.MaThanhPham) or nameof(MayCan.MaLoaiNguyenLieu);

    public static Selection Capture(MayCan machine)
    {
        var state = States.GetOrCreateValue(machine);
        lock (state)
        {
            var error = state.Pending.Count == 0 ? null : state.Pending.Values.Any(x =>
                System.Diagnostics.Stopwatch.GetElapsedTime(x.Started) >= ConfirmationWindow) ? Unconfirmed : Waiting;
            return new(machine.MaLo?.Trim(), machine.MaSize?.Trim(), machine.MaThanhPham?.Trim(),
                machine.MaLoaiNguyenLieu?.Trim(), state.Revision, error);
        }
    }

    public static string? Check(MayCan machine, Selection captured)
    {
        var current = Capture(machine);
        return current.Error ?? captured.Error ??
            (current.Revision != captured.Revision || current.Lot != captured.Lot || current.Size != captured.Size ||
             current.Product != captured.Product || current.Material != captured.Material ? Changed : null);
    }

    public static string? CheckAttendance(MayCan machine, Selection captured, string? direction) =>
        direction?.Trim() == "Ra" ? null : Check(machine, captured);

    // Called by web Set, not by firmware echo. Do not clear pending on a send exception:
    // the frame may already have reached the scale. Only its matching echo confirms it.
    public static Task SendChangeAsync(MayCan machine, string property, string value, Func<Task> send) =>
        HqOnlineWeightWriter.ForMachine(machine.Id, async () =>
        {
            if (!IsSelector(property) || value == null || value.Trim().Length > 50)
                throw new ArgumentException("Mã cân thiếu/sai độ dài.");
            var state = States.GetOrCreateValue(machine);
            lock (state)
            {
                value = value.Trim();
                if (state.Pending.TryGetValue(property, out var pending) &&
                    !string.Equals(pending.Value, value, StringComparison.OrdinalIgnoreCase) &&
                    System.Diagnostics.Stopwatch.GetElapsedTime(pending.Started) < ConfirmationWindow)
                    throw new InvalidOperationException(Waiting);
                // Even Set-to-current must be confirmed: the scale may have changed locally.
                state.Pending[property] = new(value, System.Diagnostics.Stopwatch.GetTimestamp());
                state.Revision++;
            }
            await send();
            return true;
        });

    // Caller owns the same per-machine gate as Set/Vao/LITE, through sending any echo response.
    public static IReadOnlyList<DataCMD> ApplyFirmware(MayCan machine, string connectionId, IReadOnlyList<DataCMD> data)
    {
        if (string.IsNullOrEmpty(connectionId) || machine.ConnectionId != connectionId) return Array.Empty<DataCMD>();
        var selectors = data.Where(x => IsSelector(x.PropetyName)).ToArray();
        if (selectors.Any(x => x.PropetyValue == null || x.PropetyValue.Trim().Length > 50) || selectors.GroupBy(x => x.PropetyName)
            .Any(g => g.Select(x => x.PropetyValue.Trim()).Distinct(StringComparer.Ordinal).Count() != 1))
            return Array.Empty<DataCMD>();
        var state = States.GetOrCreateValue(machine);
        var accepted = new List<DataCMD>();
        lock (state)
        {
            foreach (var item in selectors.DistinctBy(x => x.PropetyName))
            {
                var value = item.PropetyValue.Trim();
                if (state.Pending.TryGetValue(item.PropetyName, out var pending) &&
                    !string.Equals(pending.Value, value, StringComparison.OrdinalIgnoreCase)) continue;
                var property = typeof(MayCan).GetProperty(item.PropetyName)!;
                if (!string.Equals((string?)property.GetValue(machine), value, StringComparison.Ordinal))
                {
                    property.SetValue(machine, value);
                    state.Revision++;
                }
                if (state.Pending.Remove(item.PropetyName)) state.Revision++;
                accepted.Add(new DataCMD { CommandName = item.CommandName, PropetyName = item.PropetyName, PropetyValue = value });
            }
        }
        return accepted;
    }
}
