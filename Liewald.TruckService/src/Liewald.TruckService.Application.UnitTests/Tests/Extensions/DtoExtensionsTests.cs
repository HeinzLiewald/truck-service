using FluentAssertions;
using Liewald.TruckService.Application.Dtos;
using Liewald.TruckService.Application.Extensions;
using Liewald.TruckService.Application.UnitTests.Framework;
using Liewald.TruckService.Domain.Models;

namespace Liewald.TruckService.Application.UnitTests.Tests.Extensions;

public class DtoExtensionsTests
{
    [Theory]
    [AutoMoqData]
    public void ToDto_WithValidDriver_ReturnsDto(Driver driver)
    {
        var driverDto = driver.ToDto();

        driverDto.Should().NotBeNull();
        driverDto.Should().BeOfType<DriverDto>();

        driverDto.FirstName.Should().Be(driver.FirstName);
        driverDto.LastName.Should().Be(driver.LastName);
        driverDto.LocationCity.Should().Be(driver.Location.City);
        driverDto.LocationCountry.Should().Be(driver.Location.Country);
    }

    [Fact]
    public void ToDto_WithNullDriver_ReturnsNull()
    {
        Driver driver = null!;

        var driverDto = driver.ToDto();

        driverDto.Should().BeNull();
    }
}
