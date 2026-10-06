namespace Services;

public sealed class ScaleAttendanceOptions
{
    // Ngưỡng phát hiện quên quẹt, có thể điều chỉnh theo ca của đơn vị.
    public int MaxShiftHours { get; set; } = 16;
}
