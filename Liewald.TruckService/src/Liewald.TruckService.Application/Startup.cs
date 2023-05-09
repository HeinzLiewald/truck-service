using Azure.Identity;
using Liewald.TruckService.Application.Queries;
using Liewald.TruckService.Application.Queries.Implementations;
using Liewald.TruckService.Infrastructure.Options;
using Liewald.TruckService.Infrastructure.Services;
using Liewald.TruckService.Infrastructure.Services.Implementations;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Liewald.TruckService.Application;

public static class Startup
{
    public static IServiceCollection AddTruckServiceApplication(this IServiceCollection services, ConfigurationManager configuration)
    {
        string? cosmosConnectionString = configuration["CosmosDatabase:ConnectionString"];

        if (string.IsNullOrWhiteSpace(cosmosConnectionString))
        {
            services.AddSingleton(_ => new CosmosClient(configuration["CosmosDatabase:AccountEndpoint"], new DefaultAzureCredential()));
        }
        else
        {
            services.AddSingleton(_ => new CosmosClient(cosmosConnectionString));
        }

        return services
            .AddScoped<IContainerFactory, ContainerFactory>()
            .AddScoped<IDataReader, CosmosDataReader>()
            .AddScoped<IDriverQueries, DriverQueries>();
    }

    public static async Task CreateContainersAsync(this IServiceScope scope)
    {
        CosmosClient client = scope.ServiceProvider.GetRequiredService<CosmosClient>();
        IOptions<CosmosDatabaseOptions> options = scope.ServiceProvider.GetRequiredService<IOptions<CosmosDatabaseOptions>>();
        CosmosDatabaseOptions cosmosDatabaseOptions = options.Value;

        Database database = await client.CreateDatabaseIfNotExistsAsync(cosmosDatabaseOptions.DatabaseName).ConfigureAwait(false);
        await database.CreateContainerIfNotExistsAsync(cosmosDatabaseOptions.CreateContainerProperties()).ConfigureAwait(false);
    }
}
