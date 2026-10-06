using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models.Repos;

namespace Services;

/// <summary>Ghi nhận lượt Vào thiếu Ra sau khi ngày kết thúc, không tạo giờ Ra giả.</summary>
public sealed class ScaleAttendanceDayCloseService(
    ILogger<ScaleAttendanceDayCloseService> logger) : BackgroundService
{
    // Dùng UTC+7 rõ ràng, không phụ thuộc múi giờ cài trên Windows/server.
    internal static DateTime BusinessDate(DateTimeOffset utc) =>
        utc.ToOffset(TimeSpan.FromHours(7)).Date;

    internal const string CloseSql = """
        ;WITH Events AS
        (
            SELECT Id, ThoiGian, MaSoMay,
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
            SELECT Id, Direction,
                LEAD(Direction) OVER (
                    PARTITION BY Card, MaSoMay, CONVERT(date, ThoiGian)
                    ORDER BY ThoiGian, Id) AS NextDirection
            FROM Events
        )
        UPDATE c
        SET Isforget = 1
        FROM dbo.CheckInOut c
        JOIN Ordered e ON e.Id = c.Id
        WHERE e.Direction = N'Vào' AND (e.NextDirection IS NULL OR e.NextDirection <> N'Ra')
          AND COALESCE(c.Isforget, 0) <> 1;
        """;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var today = BusinessDate(DateTimeOffset.UtcNow);
            try
            {
                using var db = new dbPMScontext();
                db.Database.SetCommandTimeout(120);
                var count = await db.Database.ExecuteSqlRawAsync(
                    CloseSql, new object[] { today }, stoppingToken);
                logger.LogInformation("[CHAMCONG DAY CLOSE] Ngay={Day:yyyy-MM-dd}, phieu ghi loi={Count}",
                    today, count);
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

            // Quét bù ngay khi khởi động; sau đó chạy đúng ranh giới ngày UTC+7.
            var nextMidnight = new DateTimeOffset(today.AddDays(1), TimeSpan.FromHours(7));
            var delay = nextMidnight - DateTimeOffset.UtcNow;
            if (delay <= TimeSpan.Zero) continue;
            try { await Task.Delay(delay, stoppingToken); }
            catch (OperationCanceledException) { return; }
        }
    }
}
