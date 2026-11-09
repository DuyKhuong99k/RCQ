namespace Services;

public interface IHubEmployeeCardCatalog
{
    Task RefreshAsync(CancellationToken cancellationToken = default);
}
