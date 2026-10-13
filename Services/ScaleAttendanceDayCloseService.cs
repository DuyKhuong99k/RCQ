using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Repos;

namespace Services;

/// <summary>Ghi nhận lượt Vào thiếu Ra sau khi ngày kết thúc, không tạo giờ Ra giả.</summary>
public sealed class ScaleAttendanceDayCloseService(
    ILogger<ScaleAttendanceDayCloseService> logger,
    IOptions<ScaleAttendanceOptions> options) : BackgroundService
{
    // Dùng UTC+7 rõ ràng, không phụ thuộc múi giờ cài trên Windows/server.
    internal static DateTime BusinessDate(DateTimeOffset utc) =>
        utc.ToOffset(TimeSpan.FromHours(7)).Date;

    internal const string CloseSql = """
        ;WITH Events AS
        (
            SELECT Id, ThoiGian, ThoiGian AS ShiftStart, MaSoMay, CaDem,
                CASE WHEN CHARINDEX('.', MaChamCong) > 0
                    THEN LEFT(MaChamCong, CHARINDEX('.', MaChamCong) - 1)
                    ELSE MaChamCong END AS Card,
                TrangThai AS Direction
            FROM dbo.CheckInOut
            WHERE ThoiGian < {0}
              AND TrangThai IN (N'Vào', N'Ra')
        ),
        Ordered AS
        (
            SELECT Id, Direction, ThoiGian, ShiftStart, CaDem,
                LEAD(ThoiGian) OVER (
                    PARTITION BY Card, MaSoMay ORDER BY ThoiGian, Id) AS NextTime,
                LEAD(Direction) OVER (
                    PARTITION BY Card, MaSoMay
                    ORDER BY ThoiGian, Id) AS NextDirection
            FROM Events
        )
        UPDATE c
        SET Isforget = 1
        FROM dbo.CheckInOut c
        JOIN Ordered e ON e.Id = c.Id
        WHERE e.Direction = N'Vào'
          AND ({0} > DATEADD(HOUR, {1}, e.ShiftStart)
               OR (e.CaDem = 0 AND CONVERT(date, e.ShiftStart) < CONVERT(date, {0})))
          AND NOT (COALESCE(e.NextDirection, N'') = N'Ra'
               AND e.NextTime <= DATEADD(HOUR, {1}, e.ShiftStart)
               AND (e.CaDem = 1 OR CONVERT(date, e.NextTime) = CONVERT(date, e.ShiftStart)))
          AND COALESCE(c.Isforget, 0) <> 1;
        """;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)).DateTime;
            try
            {
                using var db = new dbPMScontext();
                db.Database.SetCommandTimeout(120);
                var count = await db.Database.ExecuteSqlRawAsync(
                    CloseSql, new object[] { now, options.Value.MaxShiftHours }, stoppingToken);
                logger.LogInformation("[CHAMCONG DAY CLOSE] Ngay={Day:yyyy-MM-dd}, phieu ghi loi={Count}",
                    now, count);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[CHAMCONG DAY CLOSE] Chưa ghi được lỗi thiếu Ra; sẽ thử lại.");
                try { await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); }
                catch (OperationCanceledException) { return; }
                continue;
            }

            // Quét bù khi khởi động và mỗi phút để phát hiện cả ca đêm hết hạn.
            try { await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); }
            catch (OperationCanceledException) { return; }
        }
    }
}
