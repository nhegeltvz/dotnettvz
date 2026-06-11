using Data.Data;
using Data.Dto.CRUD.PlayingField;
using Data.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Web.Tests;

public class PlayingFieldApiTests : IClassFixture<MatchTrackerWebApplicationFactory>, IAsyncLifetime
{
    private readonly MatchTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PlayingFieldApiTests(MatchTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    private async Task<List<PlayingField>> SeedPlayingFieldsAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MatchTrackerDbContext>();

        var fields = new List<PlayingField>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Sportski Centar Zagreb",
                Description = "An outdoor grass field in Zagreb",
                Longitude = 15.97,
                Latitude = 45.81,
                ContactNumber = "0911234567",
                Status = FieldStatus.Verified,
                IsOutdoor = true,
                SurfaceType = SurfaceType.ArtificialGrass,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Dvorana Split",
                Description = "An indoor field in Split",
                Longitude = 16.44,
                Latitude = 43.51,
                ContactNumber = "0987654321",
                Status = FieldStatus.Verified,
                IsOutdoor = false,
                SurfaceType = SurfaceType.ArtificialGrass,
            },
        };

        db.PlayingFields.AddRange(fields);
        await db.SaveChangesAsync();

        return fields;
    }

    private StadiumFormDto ValidFieldDto(string name = "New Stadium Field") => new()
    {
        Name = name,
        Description = "A valid playing field description",
        Longitude = 16.0,
        Latitude = 45.0,
        ContactNumber = "0911111111",
        Status = (int)FieldStatus.Verified,
        IsOutdoor = true,
        SurfaceType = (int)SurfaceType.NaturalGrass,
    };

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MatchTrackerDbContext>();
        db.PlayingFields.RemoveRange(db.PlayingFields.ToList());
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetById_ReturnsPlayingField_WhenPlayingFieldExists()
    {
        var fields = await SeedPlayingFieldsAsync();
        var field = fields.First();

        var response = await _client.GetAsync($"/api/playingfieldapi/{field.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<PlayingFieldDto>();
        Assert.NotNull(dto);
        Assert.Equal(field.Name, dto.Name);
    }

    [Fact]
    public async Task GetById_Returns404_WhenPlayingFieldDoesNotExist()
    {
        var response = await _client.GetAsync($"/api/playingfieldapi/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsTwoPlayingFields()
    {
        var fields = await SeedPlayingFieldsAsync();

        var response = await _client.GetAsync("/api/playingfieldapi/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dtos = await response.Content.ReadFromJsonAsync<List<PlayingFieldDto>>();
        Assert.Equal(fields.Count, dtos!.Count);
    }

    [Fact]
    public async Task Post_Returns201_WhenPlayingFieldSuccessfullyCreated()
    {
        _factory.SetAuthenticatedAdmin();
        try
        {
            var response = await _client.PostAsJsonAsync("/api/playingfieldapi/", ValidFieldDto());
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
        finally { _factory.SetAuthenticatedUser(null); }
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenFormInvalid()
    {
        _factory.SetAuthenticatedAdmin();
        try
        {
            var dto = ValidFieldDto();
            dto.Name = "ab"; // below MinimumLength of 3 — fails [StringLength(50, MinimumLength = 3)]

            var response = await _client.PostAsJsonAsync("/api/playingfieldapi/", dto);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        finally { _factory.SetAuthenticatedUser(null); }
    }

    [Fact]
    public async Task Post_WithoutAuth_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/playingfieldapi/", ValidFieldDto());
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsNoContent_WhenUpdateSuccessful()
    {
        var fields = await SeedPlayingFieldsAsync();
        var field = fields.First();
        _factory.SetAuthenticatedAdmin();
        try
        {
            var response = await _client.PutAsJsonAsync($"/api/playingfieldapi/{field.Id}", ValidFieldDto("Updated Field Name"));
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
        finally { _factory.SetAuthenticatedUser(null); }
    }

    [Fact]
    public async Task Put_ReturnsBadRequest_WhenFormInvalid()
    {
        var fields = await SeedPlayingFieldsAsync();
        var field = fields.First();
        _factory.SetAuthenticatedAdmin();
        try
        {
            var dto = ValidFieldDto();
            dto.Name = "x"; // below MinimumLength of 3

            var response = await _client.PutAsJsonAsync($"/api/playingfieldapi/{field.Id}", dto);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        finally { _factory.SetAuthenticatedUser(null); }
    }

    [Fact]
    public async Task Put_Returns404_WhenPlayingFieldDoesNotExist()
    {
        _factory.SetAuthenticatedAdmin();
        try
        {
            var response = await _client.PutAsJsonAsync($"/api/playingfieldapi/{Guid.NewGuid()}", ValidFieldDto());
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally { _factory.SetAuthenticatedUser(null); }
    }

    [Fact]
    public async Task Put_WithoutAuth_Returns401()
    {
        var fields = await SeedPlayingFieldsAsync();
        var field = fields.First();

        var response = await _client.PutAsJsonAsync($"/api/playingfieldapi/{field.Id}", ValidFieldDto("Updated Field Name"));
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenPlayingFieldDeleted()
    {
        var fields = await SeedPlayingFieldsAsync();
        var field = fields.First();

        var response = await _client.DeleteAsync($"/api/playingfieldapi/{field.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Returns404_WhenPlayingFieldDoesNotExist()
    {
        var response = await _client.DeleteAsync($"/api/playingfieldapi/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
