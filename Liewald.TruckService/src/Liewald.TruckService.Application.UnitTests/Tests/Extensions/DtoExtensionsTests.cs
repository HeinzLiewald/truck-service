using AutoFixture;
using FluentAssertions;
using Liewald.TruckService.Application.Extensions;
using Liewald.TruckService.Domain.Models;

namespace Liewald.TruckService.Application.UnitTests.Tests.Extensions;

public class DtoExtensionsTests
{
    private readonly IFixture _fixture;

    public DtoExtensionsTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void ToDto_WithValidDriver_ReturnsDto()
    {
        var driver = _fixture.Create<Driver>();

        var driverDto = driver.ToDto();

        driverDto.Should().NotBeNull();

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
