namespace Liewald.TruckService.Infrastructure.Services;

public interface IDataReader
{
    Task ReadAsync<T>(
        IQueryable<T> query,
        List<T> result,
        CancellationToken cancellationToken);
}
