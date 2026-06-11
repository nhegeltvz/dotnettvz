using Data.Data;
using Data.Dto.CRUD.Party;
using Data.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Web.Models.Dto;

namespace Web.Tests
{
    public class PartyApiTests : IClassFixture<MatchTrackerWebApplicationFactory>, IAsyncLifetime
    {
        private readonly MatchTrackerWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public PartyApiTests(MatchTrackerWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        private async Task<List<Party>> SeedPartyDataAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MatchTrackerDbContext>();

            var users = new[]
            {
                new AppUser { Id = Guid.NewGuid(), UserName = "partyowner1",  NormalizedUserName = "PARTYOWNER1",  Email = "owner1@test.com", NormalizedEmail = "OWNER1@TEST.COM", OIB = "11111111111", JMBG = "1111111111111" },
                new AppUser { Id = Guid.NewGuid(), UserName = "partymember1", NormalizedUserName = "PARTYMEMBER1", Email = "member1@test.com", NormalizedEmail = "MEMBER1@TEST.COM", OIB = "22222222222", JMBG = "2222222222222" },
                new AppUser { Id = Guid.NewGuid(), UserName = "partymember2", NormalizedUserName = "PARTYMEMBER2", Email = "member2@test.com", NormalizedEmail = "MEMBER2@TEST.COM", OIB = "33333333333", JMBG = "3333333333333" },
                new AppUser { Id = Guid.NewGuid(), UserName = "partyowner2",  NormalizedUserName = "PARTYOWNER2",  Email = "owner2@test.com", NormalizedEmail = "OWNER2@TEST.COM", OIB = "44444444444", JMBG = "4444444444444" },
                new AppUser { Id = Guid.NewGuid(), UserName = "partymember3", NormalizedUserName = "PARTYMEMBER3", Email = "member3@test.com", NormalizedEmail = "MEMBER3@TEST.COM", OIB = "55555555555", JMBG = "5555555555555" },
                new AppUser { Id = Guid.NewGuid(), UserName = "partymember4", NormalizedUserName = "PARTYMEMBER4", Email = "member4@test.com", NormalizedEmail = "MEMBER4@TEST.COM", OIB = "66666666666", JMBG = "6666666666666" },
                new AppUser { Id = Guid.NewGuid(), UserName = "partymember5", NormalizedUserName = "PARTYMEMBER5", Email = "member5@test.com", NormalizedEmail = "MEMBER5@TEST.COM", OIB = "77777777777", JMBG = "7777777777777" },
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

            var owner1 = players[0];
            var owner2 = players[3];

            var party1 = new Party
            {
                Id = Guid.NewGuid(),
                PlayerCreatedId = owner1.Id,
                DateCreated = DateTime.UtcNow,
                MaxMembers = 10,
                PartyDescription = "Test Party 1",
                PreferredLocations = "Zagreb",
                Members = new List<Player> { players[0], players[1], players[2] },
                PreferredPlayingDates = new List<PreferredPlayingDate>
                {
                    new() { Id = Guid.NewGuid(), Date = new DateTime(2026, 7, 1, 18, 0, 0, DateTimeKind.Utc) },
                    new() { Id = Guid.NewGuid(), Date = new DateTime(2026, 7, 8, 18, 0, 0, DateTimeKind.Utc) },
                },
            };

            var party2 = new Party
            {
                Id = Guid.NewGuid(),
                PlayerCreatedId = owner2.Id,
                DateCreated = DateTime.UtcNow,
                MaxMembers = 10,
                PartyDescription = "Test Party 2",
                PreferredLocations = "Split",
                Members = new List<Player> { players[3], players[4], players[5], players[6] },
                PreferredPlayingDates = new List<PreferredPlayingDate>
                {
                    new() { Id = Guid.NewGuid(), Date = new DateTime(2026, 7, 5, 20, 0, 0, DateTimeKind.Utc) },
                },
            };

            db.Parties.AddRange(party1, party2);
            await db.SaveChangesAsync();

            return new List<Party> { party1, party2 };
        }

        // Returns the AppUser.Id for a given Player.Id by querying the DB
        private Guid GetAppUserId(Guid playerId)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MatchTrackerDbContext>();
            return db.Players.First(p => p.Id == playerId).UserId;
        }

        // Returns the AppUser.Id for the first user with the given username
        private Guid GetAppUserIdByUsername(string username)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MatchTrackerDbContext>();
            return db.Users.First(u => u.UserName == username).Id;
        }

        public async Task InitializeAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MatchTrackerDbContext>();
            db.Parties.RemoveRange(db.Parties.ToList());
            db.Players.RemoveRange(db.Players.ToList());
            db.Users.RemoveRange(db.Users.ToList());
            await db.SaveChangesAsync();
            _factory.SetAuthenticatedUser(null);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetById_ReturnsParty_WhenPartyExists()
        {
            var parties = await SeedPartyDataAsync();
            var party = parties.First();

            var response = await _client.GetAsync($"/api/partyapi/{party.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var dto = await response.Content.ReadFromJsonAsync<PartyDto>();
            Assert.NotNull(dto);
            Assert.Equal(party.Id, dto.Id);
        }

        [Fact]
        public async Task Get_ReturnsParties()
        {
            var parties = await SeedPartyDataAsync();
            var response = await _client.GetAsync("/api/partyapi/");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var dtos = await response.Content.ReadFromJsonAsync<List<PartyDto>>();
            Assert.Equal(parties.Count, dtos!.Count);
        }

        [Fact]
        public async Task GetById_Returns404_WhenPartyDoesNotExist()
        {
            var response = await _client.GetAsync($"/api/partyapi/{Guid.NewGuid()}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Post_Returns201_WhenPartySuccessfullyCreated()
        {
            await SeedPartyDataAsync();
            // partymember1 is a member of party1 but does not own any party — eligible to create
            var userId = GetAppUserIdByUsername("partymember1");
            _factory.SetAuthenticatedUser(userId);
            try
            {
                var dto = new UserCreatePartyDto
                {
                    MaxMembers = 8,
                    PartyDescription = "Brand new test party",
                    PreferredLocations = "Rijeka",
                    PreferredPlayingDates = [new DateTime(2026, 8, 1, 18, 0, 0, DateTimeKind.Utc)],
                };

                var response = await _client.PostAsJsonAsync("/api/partyapi/user-create", dto);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            }
            finally { _factory.SetAuthenticatedUser(null); }
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenFormInvalid()
        {
            await SeedPartyDataAsync();
            var userId = GetAppUserIdByUsername("partymember1");
            _factory.SetAuthenticatedUser(userId);
            try
            {
                var dto = new UserCreatePartyDto
                {
                    MaxMembers = 1,          // below minimum of 2
                    PartyDescription = "x",  // below minimum of 10 chars
                    PreferredLocations = "y", // below minimum of 5 chars
                };

                var response = await _client.PostAsJsonAsync("/api/partyapi/user-create", dto);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
            finally { _factory.SetAuthenticatedUser(null); }
        }

        [Fact]
        public async Task Put_ReturnsNoContent_WhenPatchSuccessful()
        {
            var parties = await SeedPartyDataAsync();
            var party = parties.First();
            var ownerUserId = GetAppUserId(party.PlayerCreatedId);
            _factory.SetAuthenticatedUser(ownerUserId);
            try
            {
                var dto = new UserCreatePartyDto
                {
                    MaxMembers = 12,
                    PartyDescription = "Updated party description",
                    PreferredLocations = "Osijek",
                    PreferredPlayingDates = [new DateTime(2026, 9, 1, 18, 0, 0, DateTimeKind.Utc)],
                };

                var response = await _client.PutAsJsonAsync($"/api/partyapi/{party.Id}/user-edit", dto);

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
            finally { _factory.SetAuthenticatedUser(null); }
        }

        [Fact]
        public async Task Put_ReturnsBadRequest_WhenFormInvalid()
        {
            var parties = await SeedPartyDataAsync();
            var party = parties.First();
            var ownerUserId = GetAppUserId(party.PlayerCreatedId);
            _factory.SetAuthenticatedUser(ownerUserId);
            try
            {
                var dto = new UserCreatePartyDto
                {
                    MaxMembers = 0,           // below minimum of 2
                    PartyDescription = "bad", // below minimum of 10 chars
                    PreferredLocations = "x",  // below minimum of 5 chars
                };

                var response = await _client.PutAsJsonAsync($"/api/partyapi/{party.Id}/user-edit", dto);

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
            finally { _factory.SetAuthenticatedUser(null); }
        }

        [Fact]
        public async Task Put_Returns404_WhenPartyDoesNotExist()
        {
            var parties = await SeedPartyDataAsync();
            var ownerUserId = GetAppUserId(parties.First().PlayerCreatedId);
            _factory.SetAuthenticatedUser(ownerUserId);
            try
            {
                var dto = new UserCreatePartyDto
                {
                    MaxMembers = 8,
                    PartyDescription = "Valid description here",
                    PreferredLocations = "Zagreb",
                };

                var response = await _client.PutAsJsonAsync($"/api/partyapi/{Guid.NewGuid()}/user-edit", dto);

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
            finally { _factory.SetAuthenticatedUser(null); }
        }

        [Fact]
        public async Task Delete_Returns404_WhenPartyDoesNotExist()
        {
            var parties = await SeedPartyDataAsync();
            var ownerUserId = GetAppUserId(parties.First().PlayerCreatedId);
            _factory.SetAuthenticatedUser(ownerUserId);
            try
            {
                var response = await _client.DeleteAsync($"/api/partyapi/{Guid.NewGuid()}/close");

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
            finally { _factory.SetAuthenticatedUser(null); }
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenPartyDeleted()
        {
            var parties = await SeedPartyDataAsync();
            var party = parties.First();
            var ownerUserId = GetAppUserId(party.PlayerCreatedId);
            _factory.SetAuthenticatedUser(ownerUserId);
            try
            {
                var response = await _client.DeleteAsync($"/api/partyapi/{party.Id}/close");

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
            finally { _factory.SetAuthenticatedUser(null); }
        }

        // ── Business logic / auth tests ──────────────────────────────────────

        [Fact]
        public async Task Unauthenticated_TryToCreateParty_Returns401()
        {
            var dto = new UserCreatePartyDto
            {
                MaxMembers = 8,
                PartyDescription = "Some valid party description",
                PreferredLocations = "Zagreb",
            };

            var response = await _client.PostAsJsonAsync("/api/partyapi/user-create", dto);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task NonPartyOwner_TryToDeleteParty_Returns403()
        {
            var parties = await SeedPartyDataAsync();
            var party = parties.First();
            // partymember1 is a member of party1 but NOT the owner
            var nonOwnerUserId = GetAppUserIdByUsername("partymember1");
            _factory.SetAuthenticatedUser(nonOwnerUserId);
            try
            {
                var response = await _client.DeleteAsync($"/api/partyapi/{party.Id}/close");

                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            }
            finally { _factory.SetAuthenticatedUser(null); }
        }

        [Fact]
        public async Task NonPartyOwner_TryToKickPlayers_Returns403()
        {
            var parties = await SeedPartyDataAsync();
            var party = parties.First();
            // partymember1 is a member of party1 but NOT the owner
            var nonOwnerUserId = GetAppUserIdByUsername("partymember1");
            _factory.SetAuthenticatedUser(nonOwnerUserId);
            try
            {
                // Try to kick partymember2 from party1
                using var scope = _factory.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<MatchTrackerDbContext>();
                var targetPlayer = db.Players.First(p => p.User.UserName == "partymember2");

                var response = await _client.DeleteAsync($"/api/partyapi/{party.Id}/kick/{targetPlayer.Id}");

                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            }
            finally { _factory.SetAuthenticatedUser(null); }
        }
    }
}
