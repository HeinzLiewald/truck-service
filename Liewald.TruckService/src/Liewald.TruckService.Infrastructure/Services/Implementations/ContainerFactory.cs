using System.Diagnostics.CodeAnalysis;
using Liewald.TruckService.Infrastructure.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Liewald.TruckService.Infrastructure.Services.Implementations;

[ExcludeFromCodeCoverage]
public sealed class ContainerFactory : IContainerFactory
{
    private readonly CosmosDatabaseOptions _cosmosDatabaseOptions;
    private readonly CosmosClient _cosmosClient;

    public ContainerFactory(
        IOptions<CosmosDatabaseOptions> cosmosDatabaseOptions,
        CosmosClient cosmosClient)
    {
        _cosmosDatabaseOptions = cosmosDatabaseOptions.Value;
        _cosmosClient = cosmosClient;
    }

    public Container CreateDriversContainerInstance()
    {
        ArgumentNullException.ThrowIfNull(_cosmosDatabaseOptions.DatabaseName);
        ArgumentNullException.ThrowIfNull(_cosmosDatabaseOptions.DriversContainerName);

        return _cosmosClient.GetContainer(
            databaseId: _cosmosDatabaseOptions.DatabaseName,
            containerId: _cosmosDatabaseOptions.DriversContainerName);
    }
}
