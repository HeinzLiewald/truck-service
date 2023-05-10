using AutoFixture.Xunit2;
using FluentAssertions;
using Liewald.TruckService.Application.Extensions;
using Liewald.TruckService.Application.Queries.Implementations;
using Liewald.TruckService.Application.UnitTests.Framework;
using Liewald.TruckService.Domain.Models;
using Liewald.TruckService.Infrastructure.Services;
using Microsoft.Azure.Cosmos;
using Moq;

namespace Liewald.TruckService.Application.UnitTests.Tests.Queries;

public class DriverQueriesTests
{
    [Theory]
    [AutoMoqData]
    public async Task GetAsync_WithValidLocation_ReturnsDrivers(
        [Frozen] Mock<IContainerFactory> containerFactoryMock,
        [Frozen] Mock<IDataReader> dataReaderMock,
        [Frozen] Mock<Container> containerMock,
        DriverQueries sut,
        List<Driver> drivers)
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        containerFactoryMock
            .Setup(cf => cf.CreateDriversContainerInstance())
            .Returns(containerMock.Object);

        containerMock
            .Setup(c => c.GetItemLinqQueryable<Driver>(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>(), It.IsAny<CosmosLinqSerializerOptions>()))
            .Returns(drivers.AsQueryable().OrderBy(d => d.Id));

        dataReaderMock
            .Setup(dr => dr.ReadAsync(It.IsAny<IQueryable<Driver>>(), It.IsAny<List<Driver>>(), cancellationToken))
            .Callback<IQueryable<Driver>, List<Driver>, CancellationToken>((_, list, _) =>
            {
                list.AddRange(drivers);
            })
            .Returns(Task.CompletedTask);

        var expectedDrivers = drivers.Select(d => d.ToDto()).ToList();

        // Act
        var actualDrivers = await sut.GetAsync("any location", cancellationToken);

        // Assert
        actualDrivers.Should().NotBeNull();
        actualDrivers.Should().HaveSameCount(expectedDrivers);
        actualDrivers.Should().BeEquivalentTo(expectedDrivers);
    }

    [Theory]
    [AutoMoqData]
    public async Task GetAsync_WithValidLocationButNoDrivers_ReturnsEmpty(
        [Frozen] Mock<IContainerFactory> containerFactoryMock,
        [Frozen] Mock<Container> containerMock,
        DriverQueries sut)
    {
        // Arrange
        containerFactoryMock
            .Setup(cf => cf.CreateDriversContainerInstance())
            .Returns(containerMock.Object);

        containerMock
            .Setup(c => c.GetItemLinqQueryable<Driver>(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>(), It.IsAny<CosmosLinqSerializerOptions>()))
            .Returns(Enumerable.Empty<Driver>().AsQueryable().OrderBy(d => d.Id));

        // Act
        var actualDrivers = await sut.GetAsync("any location", CancellationToken.None);

        // Assert
        actualDrivers.Should().NotBeNull();
        actualDrivers.Should().BeEmpty();
    }

    [Theory]
    [InlineAutoMoqData("")]
    [InlineAutoMoqData(null!)]
    public async Task GetAsync_WithEmptyLocation_ThrowsArgumentException(
        string location,
        DriverQueries sut)
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        Func<Task> act = () => sut.GetAsync(location, cancellationToken);

        // Assert
        await act
            .Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("'location' cannot be null or whitespace. (Parameter 'location')");
    }
}
