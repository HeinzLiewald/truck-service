using Microsoft.Azure.Cosmos;

namespace Liewald.TruckService.Infrastructure.Services;

public interface IContainerFactory
{
    Container CreateDriversContainerInstance();
}