using System.Runtime.CompilerServices;
using Models.Repos.Models;

namespace Services;

/// <summary>Best-effort display only. Never queue SQL, counters or firmware messages here.</summary>
public static class HqOnlineUi
{
    public const int MaxPendingRows = 100;
    private static readonly ConditionalWeakTable<MayCan, Mailbox> Boxes = new();

    private sealed record Row(HQ_PhieuCan Item, Action<HQ_PhieuCan> Apply);

    private sealed class Mailbox
    {
        public Mailbox() { }
        public readonly Queue<Row> Rows = new();
        public Action? Notify;
        public bool Running;
        public TaskCompletionSource Idle = Completed();
        public long Dropped;

        private static TaskCompletionSource Completed()
        {
            var signal = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            signal.SetResult();
            return signal;
        }
    }

    public readonly record struct State(int PendingRows, bool Running, long DroppedDisplayRows);

    public static State Inspect(MayCan machine)
    {
        var box = Boxes.GetOrCreateValue(machine);
        lock (box) return new(box.Rows.Count, box.Running, box.Dropped);
    }

    public static Task WhenIdleAsync(MayCan machine)
    {
        var box = Boxes.GetOrCreateValue(machine);
        lock (box) return box.Idle.Task;
    }

    public static void Add(MayCan machine, HQ_PhieuCan item, Action<HQ_PhieuCan> apply) =>
        Post(machine, new(item, apply), null);

    public static void Notify(MayCan machine, Action notify) => Post(machine, null, notify);

    private static void Post(MayCan machine, Row? row, Action? notify)
    {
        var box = Boxes.GetOrCreateValue(machine);
        lock (box)
        {
            if (row != null)
            {
                if (box.Rows.Count == MaxPendingRows)
                {
                    box.Rows.Dequeue();
                    box.Dropped++;
                }
                box.Rows.Enqueue(row);
            }

            if (notify != null) box.Notify = notify; // one latest refresh, never an unbounded action list
            if (box.Running) return;
            box.Running = true;
            box.Idle = new(TaskCreationOptions.RunContinuationsAsynchronously);
            StartWorker(box, machine.Id);
        }
    }

    private static void StartWorker(Mailbox box, string machine)
    {
        // Do not retain the Hub request/transport AsyncLocal scope in background UI work.
        if (ExecutionContext.IsFlowSuppressed())
        {
            _ = Task.Run(() => Drain(box, machine));
            return;
        }

        using (ExecutionContext.SuppressFlow())
            _ = Task.Run(() => Drain(box, machine));
    }

    private static void Drain(Mailbox box, string machine)
    {
        while (true)
        {
            Row? row = null;
            Action? notify = null;
            lock (box)
            {
                if (box.Rows.TryDequeue(out var value))
                {
                    row = value;
                }
                else if (box.Notify != null)
                {
                    notify = box.Notify;
                    box.Notify = null;
                }
                else
                {
                    box.Running = false;
                    box.Idle.TrySetResult();
                    return;
                }
            }

            try
            {
                if (row != null) row.Apply(row.Item);
                else notify!();
            }
            catch (Exception ex)
            {
                HqFirmwareSend.Log($"[HQ UI ERROR] Machine={machine}; Id={row?.Item.Id}; Error={ex.GetType().Name}; SQL unchanged");
            }
        }
    }
}
