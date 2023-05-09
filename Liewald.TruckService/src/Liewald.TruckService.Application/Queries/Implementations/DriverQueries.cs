using Liewald.TruckService.Application.Dtos;
using Liewald.TruckService.Application.Extensions;
using Liewald.TruckService.Domain.Models;
using Liewald.TruckService.Infrastructure.Services;
using Microsoft.Azure.Cosmos;

namespace Liewald.TruckService.Application.Queries.Implementations;

internal sealed class DriverQueries : IDriverQueries
{
    private readonly IContainerFactory _containerFactory;
    private readonly IDataReader _dataReader;

    public DriverQueries(
        IContainerFactory containerFactory,
        IDataReader dataReader)
    {
        _containerFactory = containerFactory;
        _dataReader = dataReader;
    }

    public async Task<IEnumerable<DriverDto>> GetAsync(
        string location,
        CancellationToken cancellationToken)
    {
        GuardLocationIsNotEmpty(location);

        Container container = _containerFactory.CreateDriversContainerInstance();

        IQueryable<Driver> query = container
            .GetItemLinqQueryable<Driver>()
            .Where(e => e.Location.City == location);

        List<Driver> drivers = new();

        await _dataReader.ReadAsync(query, drivers, cancellationToken).ConfigureAwait(false);

        return drivers.Select(d => d.ToDto());
    }

    private static void GuardLocationIsNotEmpty(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            throw new ArgumentException($"'{nameof(location)}' cannot be null or whitespace.", nameof(location));
        }
    }
}
