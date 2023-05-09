using Liewald.TruckService.Application.Dtos;

namespace Liewald.TruckService.Application.Queries;

public interface IDriverQueries
{
    Task<IEnumerable<DriverDto>> GetAsync(
        string location,
        CancellationToken cancellationToken);
}
