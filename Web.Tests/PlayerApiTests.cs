using Data.Data;
using Data.Data.Common;
using Data.Dto.CRUD.Player;
using Data.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Web.Tests;

public class PlayerApiTests : IClassFixture<MatchTrackerWebApplicationFactory>, IAsyncLifetime
{
    private readonly MatchTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PlayerApiTests(MatchTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    // Seeds 3 AppUsers + 3 Players and returns the Player list
    private async Task<List<Player>> SeedPlayersAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MatchTrackerDbContext>();

        var users = new[]
        {
            new AppUser { Id = Guid.NewGuid(), UserName = "testplayer1", NormalizedUserName = "TESTPLAYER1", Email = "test1@test.com", NormalizedEmail = "TEST1@TEST.COM", OIB = "12345678901", JMBG = "1234567890123" },
            new AppUser { Id = Guid.NewGuid(), UserName = "testplayer2", NormalizedUserName = "TESTPLAYER2", Email = "test2@test.com", NormalizedEmail = "TEST2@TEST.COM", OIB = "12345678902", JMBG = "1234567890124" },
            new AppUser { Id = Guid.NewGuid(), UserName = "testplayer3", NormalizedUserName = "TESTPLAYER3", Email = "test3@test.com", NormalizedEmail = "TEST3@TEST.COM", OIB = "12345678903", JMBG = "1234567890125" },
        };

        db.Users.AddRange(users);
        await db.SaveChangesAsync();

        var players = users.Select(u => new Player
        {
            Id = Guid.NewGuid(),
            UserId = u.Id,
            Bio = "A bio that is long enough to pass validation here",
            PreferredPosition = Position.Midfielder,
            DateOfBirth = new DateOnly(1995, 1, 1),
        }).ToList();

        db.Players.AddRange(players);
        await db.SaveChangesAsync();

        // Re-load with User navigation populated
        return db.Players.ToList();
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MatchTrackerDbContext>();
        db.Players.RemoveRange(db.Players.ToList());
        db.Users.RemoveRange(db.Users.ToList());
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetById_ReturnsPlayer_WhenPlayerExists()
    {
        var players = await SeedPlayersAsync();
        var player = players.First();

        var response = await _client.GetAsync($"/api/playerapi/{player.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<PlayerDto>();
        Assert.NotNull(dto);
        Assert.Equal(player.Id, dto.Id);
    }

    [Fact]
    public async Task GetById_Returns404_WhenPlayerDoesNotExist()
    {
        var response = await _client.GetAsync($"/api/playerapi/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnThreePlayers()
    {
        var players = await SeedPlayersAsync();
        var response = await _client.GetAsync("/api/playerapi/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dtos = await response.Content.ReadFromJsonAsync<List<PlayerDto>>();
        Assert.Equal(players.Count, dtos!.Count);
    }

    [Fact]
    public async Task Post_WithoutAuth_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/playerapi/", new PlayerFormDto
        {
            UserId = Guid.NewGuid(),
            Bio = "A bio that is long enough to pass validation",
            PreferredPosition = 0,
            DateOfBirth = new DateOnly(1995, 1, 1),
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Put_WithoutAuth_Returns401()
    {
        var players = await SeedPlayersAsync();
        var player = players.First();

        var response = await _client.PutAsJsonAsync($"/api/playerapi/{player.Id}", new PlayerFormDto
        {
            UserId = player.UserId,
            Bio = "Updated bio that is long enough to pass validation",
            PreferredPosition = 1,
            DateOfBirth = new DateOnly(1995, 6, 15),
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Put_NonExistingId_Returns401_WhenNotAuthenticated()
    {
        var response = await _client.PutAsJsonAsync($"/api/playerapi/{Guid.NewGuid()}", new PlayerFormDto
        {
            UserId = Guid.NewGuid(),
            Bio = "A bio that is long enough to pass validation",
            PreferredPosition = 0,
            DateOfBirth = new DateOnly(1995, 1, 1),
        });

        // Auth checked before existence — returns 401
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WithoutAuth_Returns401()
    {
        var players = await SeedPlayersAsync();
        var player = players.First();

        var response = await _client.DeleteAsync($"/api/playerapi/{player.Id}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NonExisting_Returns401_WhenNotAuthenticated()
    {
        var response = await _client.DeleteAsync($"/api/playerapi/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
