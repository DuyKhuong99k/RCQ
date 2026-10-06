using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models.Repos;

namespace Services;

/// <summary>Đánh dấu Vào thiếu Ra khi kết thúc ngày UTC+7, quét bù khi khởi động.</summary>
public sealed class WorkshopAttendanceDayCloseService(
    ILogger<WorkshopAttendanceDayCloseService> logger) : BackgroundService
{
    internal const string CloseSql = """
        ;WITH Ordered AS
        (
            SELECT Id, TrangThai,
                LEAD(TrangThai) OVER (
                    PARTITION BY MaTheTu, CONVERT(date, ThoiGian)
                    ORDER BY ThoiGian, Id) AS NextDirection
            FROM dbo.ChamCongTaiXuong
            WHERE ThoiGian < {0} AND TrangThai IN (N'Vào', N'Ra')
        )
        UPDATE c SET Isforget = 1
        FROM dbo.ChamCongTaiXuong c JOIN Ordered e ON e.Id = c.Id
        WHERE e.TrangThai = N'Vào' AND (e.NextDirection IS NULL OR e.NextDirection <> N'Ra')
          AND COALESCE(c.Isforget, 0) <> 1;
        """;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var today = ScaleAttendanceDayCloseService.BusinessDate(DateTimeOffset.UtcNow);
            try
            {
                using var db = new dbPMScontext();
                db.Database.SetCommandTimeout(120);
                var count = await db.Database.ExecuteSqlRawAsync(CloseSql, new object[] { today }, stoppingToken);
                logger.LogInformation("[WORKSHOP DAY CLOSE] Ngay={Day:yyyy-MM-dd}, phieu thieu Ra={Count}", today, count);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { return; }
            catch (Exception ex)
            {
                logger.LogError(ex, "[WORKSHOP DAY CLOSE] Chưa đánh dấu được lỗi thiếu Ra; sẽ thử lại.");
                try { await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); }
                catch (OperationCanceledException) { return; }
                continue;
            }

            var delay = new DateTimeOffset(today.AddDays(1), TimeSpan.FromHours(7)) - DateTimeOffset.UtcNow;
            if (delay <= TimeSpan.Zero) continue;
            try { await Task.Delay(delay, stoppingToken); }
            catch (OperationCanceledException) { return; }
        }
    }
}
