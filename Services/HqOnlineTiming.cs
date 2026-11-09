using System.Diagnostics;

namespace Services;

/// <summary>Only log slow HQ requests; no payload, credentials, or per-ticket synchronous disk writes.</summary>
public sealed class HqOnlineTiming(string machine, string id) : IDisposable
{
    private readonly long started = Stopwatch.GetTimestamp();
    public double QueueMs { get; set; }
    public double SqlMs { get; set; }
    public double TotalsMs { get; set; }
    public double IdentityMs { get; set; }
    public string Outcome { get; set; } = "UNCONFIRMED";
    public void Dispose()
    {
        var total = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
        if (total >= 2000)
            HqFirmwareSend.Log(FormattableString.Invariant($"[HQ ONLINE SLOW] Machine={machine}; Id={id}; Outcome={Outcome}; TotalMs={total:F0}; QueueMs={QueueMs:F0}; SqlMs={SqlMs:F0}; TotalsMs={TotalsMs:F0}; IdentityMs={IdentityMs:F0}"));
    }
}
