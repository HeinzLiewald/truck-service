using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Cosmos;

namespace Liewald.TruckService.Infrastructure.Options;

public class CosmosDatabaseOptions
{
    public const string ConfigurationSectionKey = "CosmosDatabase";

    [Required]
    public string DatabaseName { get; set; } = string.Empty;

    [Required]
    public string DriversContainerName { get; set; } = string.Empty;

    [Required]
    public string DriversPartitionKeyPath { get; set; } = string.Empty;

    public ContainerProperties CreateContainerProperties()
    {
        return new ContainerProperties(id: DriversContainerName, partitionKeyPath: DriversPartitionKeyPath);
    }
}
