using System.Diagnostics;
using Models.Repos.Models;
using Newtonsoft.Json.Linq;

namespace Services;

/// <summary>Network deadlines only. Never pass these tokens into SQL or retry a protocol frame.</summary>
public static class HqFirmwareSend
{
    public static readonly TimeSpan IdentityBudget = TimeSpan.FromSeconds(2);
    public static readonly TimeSpan ReplyBudget = TimeSpan.FromSeconds(3);
    private static readonly AsyncLocal<Scope?> Ambient = new();

    public static Scope? Current => Ambient.Value;
    public sealed record Result(bool Sent, string? Reason, double Milliseconds);
    public sealed record Failure(string Target, string Reason);

    public sealed class Scope : IDisposable
    {
        private readonly Scope? _previous;
        private Failure? _failure;
        private int _replyStarted;

        public string ConnectionId { get; }
        public CancellationToken ConnectionAborted { get; }
        public Failure? SendFailure => _failure;

        internal Scope(string connectionId, CancellationToken aborted)
        {
            _previous = Ambient.Value;
            ConnectionId = connectionId;
            ConnectionAborted = aborted;
            Ambient.Value = this;
        }

        public bool TryBeginReply() => Interlocked.Exchange(ref _replyStarted, 1) == 0;

        public void Fail(string target, string reason) =>
            Interlocked.CompareExchange(ref _failure, new(target, reason), null);

        public void Dispose()
        {
            if (ReferenceEquals(Ambient.Value, this)) Ambient.Value = _previous;
        }
    }

    public static Scope Begin(string connectionId, CancellationToken aborted) => new(connectionId, aborted);

    public static async Task<Result> RunAsync(Func<CancellationToken, Task> send, TimeSpan budget,
        CancellationToken aborted = default)
    {
        if (budget <= TimeSpan.Zero || budget > ReplyBudget)
            throw new ArgumentOutOfRangeException(nameof(budget));

        var started = Stopwatch.GetTimestamp();
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(aborted);
        deadline.CancelAfter(budget);
        try
        {
            deadline.Token.ThrowIfCancellationRequested();
            var operation = send(deadline.Token);
            // A transport/fake may ignore cancellation. Bound this caller, and observe late faults.
            _ = operation.ContinueWith(t => { _ = t.Exception; }, CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            await operation.WaitAsync(deadline.Token);
            return new(true, null, Stopwatch.GetElapsedTime(started).TotalMilliseconds);
        }
        catch (Exception ex)
        {
            var reason = aborted.IsCancellationRequested ? "DISCONNECTED" :
                deadline.IsCancellationRequested ? "TIMEOUT" : ex.GetType().Name;
            try { deadline.Cancel(); }
            catch { /* Cancellation callbacks must not turn a send failure into a SQL failure. */ }
            return new(false, reason, Stopwatch.GetElapsedTime(started).TotalMilliseconds);
        }
    }

    public static async Task<bool> IdentityAsync(MayCan machine, Func<string, CancellationToken, Task<bool>> send)
    {
        var scope = Current;
        var connection = scope?.ConnectionId ?? machine.ConnectionId ?? "";
        var accepted = false;
        var result = await RunAsync(async token => { accepted = await send(connection, token); },
            IdentityBudget, scope?.ConnectionAborted ?? CancellationToken.None);
        if (result.Sent && accepted) return true;

        var reason = result.Reason ?? "TARGET_UNAVAILABLE";
        scope?.Fail("SETNHANVIEN", reason);
        Log($"[HQ IDENTITY UNCONFIRMED] Machine={machine.Id}; Connection={connection}; Reason={reason}; SendMs={result.Milliseconds:F0}");
        return false;
    }

    public static string SaveStatus(string envelope)
    {
        try
        {
            var outer = JObject.Parse(envelope);
            var inner = outer.Value<string>("Data");
            if (string.IsNullOrWhiteSpace(inner)) inner = outer.Value<string>("DataError");
            var detail = inner == null ? null : JObject.Parse(inner);
            var status = detail?.Value<string>("SaveStatus");
            if (status == null && (detail?.Value<bool?>("IsValid") == true || detail?.Value<bool?>("AlreadyRecorded") == true))
            {
                // Attendance response, audit only; never change its payload.
                return HqOnlineResponse.Saved;
            }

            return status is HqOnlineResponse.Saved or HqOnlineResponse.NotSaved ? status : HqOnlineResponse.Unknown;
        }
        catch { return HqOnlineResponse.Unknown; }
    }

    public static void Log(string message)
    {
        try { Console.WriteLine(message); }
        catch { /* Diagnostics must not change the committed outcome or resend a frame. */ }
    }
}
