using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Cosmos.Linq;

namespace Liewald.TruckService.Infrastructure.Services.Implementations;

[ExcludeFromCodeCoverage]
public class CosmosDataReader : IDataReader
{
    public async Task ReadAsync<T>(
        IQueryable<T> query,
        List<T> result,
        CancellationToken cancellationToken)
    {
        using var feedIterator = query.ToFeedIterator();

        while (feedIterator.HasMoreResults)
        {
            foreach (var item in await feedIterator.ReadNextAsync(cancellationToken).ConfigureAwait(false))
            {
                result.Add(item);
            }
        }
    }
}
