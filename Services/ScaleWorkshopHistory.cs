using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;

namespace Services;

public static class ScaleWorkshopHistory
{
    // Never infer an old workshop from today's MayCan configuration.
    public static string? Resolve(dbPMScontext db, string machine, DateTime time) =>
        db.LichSuXuongMayCans.AsNoTracking().Where(x => x.MayCan == machine && x.TuThoiGian <= time)
            .OrderByDescending(x => x.TuThoiGian).Select(x => x.MaXuong).FirstOrDefault();

    public static string? Resolve(IEnumerable<LichSuXuongMayCan> history, string machine, DateTime time) =>
        history.Where(x => x.MayCan == machine && x.TuThoiGian <= time)
            .OrderByDescending(x => x.TuThoiGian).Select(x => x.MaXuong).FirstOrDefault();

    public static DateTime BusinessNow() => DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7)).DateTime;
}
