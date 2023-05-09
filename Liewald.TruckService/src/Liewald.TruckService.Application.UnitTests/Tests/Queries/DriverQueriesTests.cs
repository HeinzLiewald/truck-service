using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Liewald.TruckService.Application.Extensions;
using Liewald.TruckService.Application.Queries.Implementations;
using Liewald.TruckService.Domain.Models;
using Liewald.TruckService.Infrastructure.Services;
using Microsoft.Azure.Cosmos;
using Moq;

namespace Liewald.TruckService.Application.UnitTests.Tests.Queries;

public class DriverQueriesTests
{
    private readonly IFixture _fixture;

    private readonly Mock<IContainerFactory> _containerFactor;
    private readonly Mock<IDataReader> _dataReade;
    private readonly Mock<Container> _container;

    private readonly DriverQueries _sut;

    public DriverQueriesTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _containerFactor = _fixture.Freeze<Mock<IContainerFactory>>();
        _dataReade = _fixture.Freeze<Mock<IDataReader>>();
        _container = _fixture.Freeze<Mock<Container>>();

        _sut = _fixture.Create<DriverQueries>();
    }

    [Fact]
    public async Task GetAsync_WithValidLocation_ReturnsDriverDtos()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var drivers = _fixture.CreateMany<Driver>(count: 3).ToList();

        _containerFactor
            .Setup(cf => cf.CreateDriversContainerInstance())
            .ReturnsUsingFixture(_fixture);

        _container
          .Setup(c => c.GetItemLinqQueryable<Driver>(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>(), It.IsAny<CosmosLinqSerializerOptions>()))
          .Returns(drivers.AsQueryable().OrderBy(d => d.Id));

        _dataReade
            .Setup(dr => dr.ReadAsync(It.IsAny<IQueryable<Driver>>(), It.IsAny<List<Driver>>(), cancellationToken))
            .Callback<IQueryable<Driver>, List<Driver>, CancellationToken>((_, list, _) =>
            {
                list.AddRange(drivers);
            })
            .Returns(Task.CompletedTask);

        var expectedDrivers = drivers.Select(d => d.ToDto()).ToList();

        // Act
        var actualDrivers = await _sut.GetAsync("any location", cancellationToken);

        // Assert
        actualDrivers.Should().NotBeNull();
        actualDrivers.Should().HaveSameCount(expectedDrivers);
        actualDrivers.Should().BeEquivalentTo(expectedDrivers);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetAsync_WithEmptyLocation_ThrowsArgumentException(string location)
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        Func<Task> act = () => _sut.GetAsync(location, cancellationToken);

        // Assert
        await act
            .Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("'location' cannot be null or whitespace. (Parameter 'location')");
    }
}
