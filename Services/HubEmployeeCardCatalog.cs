using Dapper;
using Microsoft.Data.SqlClient;
using Models.Repos.Models;
using MvvmHelpers;
using ViewModels.Repos.Hubs.IServices;

namespace Services;

/// <summary>Refresh on operator actions, never on the weighing hot path. No firmware commands.</summary>
public class HubEmployeeCardCatalog(IMainService mainService) : IHubEmployeeCardCatalog
{
    private static readonly TimeSpan RefreshTimeout = TimeSpan.FromSeconds(15);
    private const int QueryTimeoutSeconds = 10;
    private const string CatalogQuery = """
        SELECT * FROM dbo.NhanVienDaiThanh;
        SELECT t.*, nv.Name AS NhanVienName, nv.MaHoSo AS MaSo
        FROM dbo.TheTu AS t
        LEFT JOIN dbo.NhanVienDaiThanh AS nv ON nv.MaNhanVien = t.MaNhanVien;
        """;
    private readonly SemaphoreSlim _refreshGate = new(1, 1);

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(RefreshTimeout);
        await _refreshGate.WaitAsync(timeout.Token);
        try
        {
            var (employees, cards) = await ReadAsync(timeout.Token);
            timeout.Token.ThrowIfCancellationRequested();
            var newEmployees = new ObservableRangeCollection<NhanVienDaiThanh>(employees);
            var newCards = new ObservableRangeCollection<TheTu>(cards);
            // Publish only after BOTH queries succeed. Existing readers keep their complete old list;
            // Clear()/AddRange() around SQL would temporarily report unregistered cards/employees.
            mainService.VmNhanVien.Items = newEmployees;
            mainService.VmThe.Items = newCards;
        }
        finally
        {
            _refreshGate.Release();
        }
    }

    protected virtual async Task<(List<NhanVienDaiThanh> Employees, List<TheTu> Cards)> ReadAsync(CancellationToken cancellation)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync(cancellation);
        using var results = await connection.QueryMultipleAsync(new CommandDefinition(
            CatalogQuery, commandTimeout: QueryTimeoutSeconds, cancellationToken: cancellation));
        var employees = (await results.ReadAsync<NhanVienDaiThanh>()).ToList();
        var cards = (await results.ReadAsync<TheTu>()).ToList();
        return (employees, cards);
    }

    protected virtual SqlConnection CreateConnection() => new(AppViewModels.Base.Ins.ConnectionString);
}
