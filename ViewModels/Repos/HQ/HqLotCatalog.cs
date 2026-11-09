using Models.Repos.Models;

namespace ViewModels.Repos.HQ;

/// <summary>Operator selection, not the date-based incremental firmware synchronization.</summary>
public static class HqLotCatalog
{
    public static IQueryable<string> ActiveIds(IQueryable<HQ_Lo> lots) =>
        lots.Where(lot => lot.SuDung)
            .OrderByDescending(lot => lot.MNgay)
            .ThenBy(lot => lot.Id)
            .Select(lot => lot.Id);
}
