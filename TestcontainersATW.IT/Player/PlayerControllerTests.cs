using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestcontainersATW.DataTransfer;
using TestcontainersATW.Errors;
using TestcontainersATW.Persistence;

namespace TestcontainersATW.IT.Player;

public class PlayerControllerTests : IClassFixture<AzurTechWinterApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly AzurTechWinterContext _azurTechWinterContext;

    public PlayerControllerTests(AzurTechWinterApiFactory factory)
    {
        _client = factory.CreateClient();
        _azurTechWinterContext =
            factory.Services.CreateScope().ServiceProvider.GetRequiredService<AzurTechWinterContext>();
    }

    [Fact]
    public async Task CreatePlayer_ShouldSavePlayerInDatabase_WhenPostRouteIsCalled()
    {
        // Arrange
        var transferObject = new PlayerDataTransferObject
        {
            Name = "Azur",
            HealthPoints = 100,
            Strength = 4
        };

        // Act
        var response = await _client.PostAsJsonAsync("/player", transferObject);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<Entities.Player>();

        result.Should().NotBeNull();
        result!.Name.Should().Be(transferObject.Name);

        var saved = await _azurTechWinterContext.Players.Where(player => player.Name == transferObject.Name).SingleAsync();
        saved.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task CreatePlayer_ShouldReturnBadRequest_WhenPostRouteIsCalledWithSameName()
    {
        // Arrange
        var winter1 = new PlayerDataTransferObject
        {
            Name = "Winter",
            HealthPoints = 100,
            Strength = 4
        };

        var winter2 = new PlayerDataTransferObject
        {
            Name = "Winter",
            HealthPoints = 95,
            Strength = 2
        };

        // Act
        await _client.PostAsJsonAsync("/player", winter1);
        var response = await _client.PostAsJsonAsync("/player", winter2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Content.ReadFromJsonAsync<UniqueNameViolationError>();

        result.Should().NotBeNull();
        result!.MessageText.Should().Contain("duplicate key value violates unique constraint");

        var saved = await _azurTechWinterContext.Players.ToListAsync();
        saved.Should().HaveCount(1);

        var single = saved.Single();
        single.Should().BeEquivalentTo(new Entities.Player()
        {
            Id = single.Id,
            Name = winter1.Name,
            HealthPoints = winter1.HealthPoints,
            Strength = winter1.Strength
        });
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        foreach (var player in await _azurTechWinterContext.Players.ToListAsync())
        {
            _azurTechWinterContext.Remove(player);
        }

        await _azurTechWinterContext.SaveChangesAsync();
    }
}