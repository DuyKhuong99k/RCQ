using AppViewModels;
using Microsoft.Data.SqlClient;

namespace Services;

public static class HqDatabaseStartup
{
    // Must run before building/resolving hosted services, not lazily on the first browser/counter connection.
    public static async Task InitializeAsync(string contentRoot, CancellationToken cancellation = default)
    {
        var file = Path.Combine(contentRoot, "wwwroot", "JsonFuFunc", "DatabaseConnectSetting.json");
        if (!File.Exists(file)) throw new InvalidOperationException("Thiếu DatabaseConnectSetting.json. Hub chưa khởi động.");
        Directory.SetCurrentDirectory(contentRoot);
        _ = AppViewModel.Instance;
        Validate(Base.Ins.ConnectionString, Base.Ins.ConnectionString2);
        await ProbeAsync(Base.Ins.ConnectionString2!, cancellation);
    }

    public static void Validate(string? daoConnection, string? efConnection)
    {
        if (string.IsNullOrWhiteSpace(daoConnection) || string.IsNullOrWhiteSpace(efConnection))
            throw new InvalidOperationException("Chưa nạp cấu hình SQL. Hub chưa khởi động.");
        var dao = new SqlConnectionStringBuilder(daoConnection);
        var ef = new SqlConnectionStringBuilder(efConnection);
        if (string.IsNullOrWhiteSpace(ef.DataSource) || string.IsNullOrWhiteSpace(ef.InitialCatalog) ||
            (!ef.IntegratedSecurity && string.IsNullOrWhiteSpace(ef.UserID)) ||
            !dao.EquivalentTo(ef))
            throw new InvalidOperationException("Cấu hình SQL thiếu hoặc không thống nhất. Hub chưa khởi động.");
    }

    public static async Task ProbeAsync(string connection, CancellationToken cancellation = default)
    {
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
        deadline.CancelAfter(TimeSpan.FromSeconds(10));
        var settings = new SqlConnectionStringBuilder(connection) { ConnectTimeout = 10, Pooling = false };
        await using var sql = new SqlConnection(settings.ConnectionString);
        try
        {
            await sql.OpenAsync(deadline.Token);
            await using var command = sql.CreateCommand();
            command.CommandText = "SELECT 1";
            command.CommandTimeout = 5;
            await command.ExecuteScalarAsync(deadline.Token);
        }
        catch (Exception ex) when (ex is SqlException or OperationCanceledException)
        {
            // No connection string/password in the startup exception.
            throw new InvalidOperationException("Không kết nối được SQL. Kiểm tra cấu hình/quyền; Hub chưa nhận dữ liệu.");
        }
    }
}
