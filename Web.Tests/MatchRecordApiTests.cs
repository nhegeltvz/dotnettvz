using Data.Data;
using Data.Dto.CRUD.MatchRecord;
using Data.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Web.Tests;

public class MatchRecordApiTests : IClassFixture<MatchTrackerWebApplicationFactory>, IAsyncLifetime
{
    private readonly MatchTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public MatchRecordApiTests(MatchTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    private async Task<(List<MatchRecord> Records, PlayingField Field)> SeedMatchRecordsAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MatchTrackerDbContext>();

        var field = new PlayingField
        {
            Id = Guid.NewGuid(),
            Name = "Test Field",
            Description = "A test playing field",
            Longitude = 15.97,
            Latitude = 45.81,
            ContactNumber = "0911234567",
            Status = FieldStatus.Verified,
            IsOutdoor = true,
            SurfaceType = SurfaceType.ArtificialGrass,
        };

        db.PlayingFields.Add(field);
        await db.SaveChangesAsync();

        var records = new List<MatchRecord>
        {
            new()
            {
                Id = Guid.NewGuid(),
                PlayingFieldId = field.Id,
                WasMatchHeld = true,
                MatchHeld = new DateTime(2026, 6, 1, 18, 0, 0, DateTimeKind.Utc),
                GoalsTeamA = 3,
                GoalsTeamB = 1,
            },
            new()
            {
                Id = Guid.NewGuid(),
                PlayingFieldId = field.Id,
                WasMatchHeld = true,
                MatchHeld = new DateTime(2026, 6, 8, 18, 0, 0, DateTimeKind.Utc),
                GoalsTeamA = 2,
                GoalsTeamB = 2,
            },
        };

        db.MatchRecords.AddRange(records);
        await db.SaveChangesAsync();

        return (records, field);
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MatchTrackerDbContext>();
        db.MatchRecords.RemoveRange(db.MatchRecords.ToList());
        db.PlayingFields.RemoveRange(db.PlayingFields.ToList());
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetById_ReturnsMatchRecord_WhenMatchRecordExists()
    {
        var (records, _) = await SeedMatchRecordsAsync();
        var record = records.First();

        var response = await _client.GetAsync($"/api/matchrecordapi/{record.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<MatchRecordDto>();
        Assert.NotNull(dto);
        Assert.Equal(record.Id, dto.Id);
    }

    [Fact]
    public async Task GetById_Returns404_WhenMatchRecordDoesNotExist()
    {
        var response = await _client.GetAsync($"/api/matchrecordapi/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsTwoMatchRecords()
    {
        var (records, _) = await SeedMatchRecordsAsync();

        var response = await _client.GetAsync("/api/matchrecordapi/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dtos = await response.Content.ReadFromJsonAsync<List<MatchRecordDto>>();
        Assert.Equal(records.Count, dtos!.Count);
    }

    [Fact]
    public async Task Post_Returns201_WhenMatchRecordSuccessfullyCreated()
    {
        var (_, field) = await SeedMatchRecordsAsync();
        _factory.SetAuthenticatedAdmin();
        try
        {
            var dto = new MatchRecordFormDto
            {
                WasMatchHeld = true,
                MatchHeld = new DateTime(2026, 5, 15, 18, 0, 0, DateTimeKind.Utc),
                PlayingFieldId = field.Id,
                GoalsTeamA = 2,
                GoalsTeamB = 1,
            };

            var response = await _client.PostAsJsonAsync("/api/matchrecordapi/", dto);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
        finally { _factory.SetAuthenticatedUser(null); }
    }

    [Fact]
    public async Task Post_WithoutAuth_Returns401()
    {
        var (_, field) = await SeedMatchRecordsAsync();

        var dto = new MatchRecordFormDto
        {
            WasMatchHeld = true,
            MatchHeld = new DateTime(2026, 5, 15, 18, 0, 0, DateTimeKind.Utc),
            PlayingFieldId = field.Id,
            GoalsTeamA = 2,
            GoalsTeamB = 1,
        };

        var response = await _client.PostAsJsonAsync("/api/matchrecordapi/", dto);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenFormInvalid()
    {
        var (_, field) = await SeedMatchRecordsAsync();
        _factory.SetAuthenticatedAdmin();
        try
        {
            var dto = new MatchRecordFormDto
            {
                WasMatchHeld = true,
                MatchHeld = new DateTime(2026, 5, 15, 18, 0, 0, DateTimeKind.Utc),
                PlayingFieldId = field.Id,
                GoalsTeamA = -1, // invalid — below Range(0, 100)
                GoalsTeamB = -1,
            };

            var response = await _client.PostAsJsonAsync("/api/matchrecordapi/", dto);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        finally { _factory.SetAuthenticatedUser(null); }
    }

    [Fact]
    public async Task Put_ReturnsNoContent_WhenUpdateSuccessful()
    {
        var (records, field) = await SeedMatchRecordsAsync();
        var record = records.First();

        var dto = new MatchRecordFormDto
        {
            WasMatchHeld = true,
            MatchHeld = new DateTime(2026, 6, 1, 18, 0, 0, DateTimeKind.Utc),
            PlayingFieldId = field.Id,
            GoalsTeamA = 5,
            GoalsTeamB = 0,
        };

        var response = await _client.PutAsJsonAsync($"/api/matchrecordapi/{record.Id}", dto);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsBadRequest_WhenFormInvalid()
    {
        var (records, field) = await SeedMatchRecordsAsync();
        var record = records.First();

        var dto = new MatchRecordFormDto
        {
            WasMatchHeld = true,
            MatchHeld = new DateTime(2026, 6, 1, 18, 0, 0, DateTimeKind.Utc),
            PlayingFieldId = field.Id,
            GoalsTeamA = -3, // invalid
            GoalsTeamB = -3,
        };

        var response = await _client.PutAsJsonAsync($"/api/matchrecordapi/{record.Id}", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_Returns404_WhenMatchRecordDoesNotExist()
    {
        var (_, field) = await SeedMatchRecordsAsync();

        var dto = new MatchRecordFormDto
        {
            WasMatchHeld = true,
            MatchHeld = new DateTime(2026, 6, 1, 18, 0, 0, DateTimeKind.Utc),
            PlayingFieldId = field.Id,
            GoalsTeamA = 1,
            GoalsTeamB = 1,
        };

        var response = await _client.PutAsJsonAsync($"/api/matchrecordapi/{Guid.NewGuid()}", dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenMatchRecordDeleted()
    {
        var (records, _) = await SeedMatchRecordsAsync();
        var record = records.First();

        var response = await _client.DeleteAsync($"/api/matchrecordapi/{record.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Returns404_WhenMatchRecordDoesNotExist()
    {
        var response = await _client.DeleteAsync($"/api/matchrecordapi/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
