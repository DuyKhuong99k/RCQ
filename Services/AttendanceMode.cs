using System.Text.Json.Serialization;

namespace Services;

/// <summary>One immutable mode snapshot per operation; production never reads the settings file.</summary>
public sealed record AttendanceModeState([property: JsonRequired] bool Enabled, long Revision, int? ChangedById,
    string? ChangedBy, DateTimeOffset? ChangedAtUtc)
{
    public static AttendanceModeState Default { get; } = new(true, 0, null, null, null);
}

public interface IAttendanceMode
{
    AttendanceModeState Current { get; }
    event Action? Changed;
}

public sealed record AttendanceModeResult(bool Success, string Message);

public static class AttendanceModeMessages
{
    public const string Disabled = "Chấm công đang tắt";
    public const string DisabledCode = "ATTENDANCE_DISABLED";
    public const string TransferDisabled = "Chấm công đang tắt. Không chuyển ca.";
}
