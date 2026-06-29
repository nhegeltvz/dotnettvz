using Data.Data;
using Data.Models;
using Data.Services.Stores;
using Microsoft.AspNetCore.Identity;

namespace Web
{
    public class DataSeeder
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly StadiumStore _stadiumStore;
        private readonly PlayerStore _playerStore;
        private readonly MatchStore _matchStore;
        private readonly PartyStore _partyStore;
        private readonly MatchTrackerDbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public DataSeeder(UserManager<AppUser> userManager, StadiumStore stadiumStore, PlayerStore playerStore, MatchStore matchStore, PartyStore partyStore, MatchTrackerDbContext dbContext, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _stadiumStore = stadiumStore;
            _playerStore = playerStore;
            _matchStore = matchStore;
            _partyStore = partyStore;
            _dbContext = dbContext;
            _env = env;
        }

        public async Task SeedAsync()
        {
            if (_userManager.Users.Any()) return;

            await SeedUsersAndAdminAsync();
            await SeedPlayingFieldsAsync();
            await SeedPlayersAsync();
            await SeedPartiesAsync();
        }

        private async Task SeedUsersAndAdminAsync()
        {
            var admin = new AppUser { UserName = "admin", Email = "admin@admin.com" };
            await _userManager.CreateAsync(admin, "Admin123!");
            await _userManager.AddToRoleAsync(admin, "Admin");

            var users = new[]
            {
                ("dlivakovic", "dlivakovic@tvz.hr", "Dlivakovic123!"),
                ("jstanisic",  "jstanisic@tvz.hr",  "Jstanisic123!"),
                ("jsutalo",    "jsutalo@tvz.hr",    "Jsutalo123!"),
                ("jgvardiol",  "jgvardiol@tvz.hr",  "Jgvardiol123!"),
                ("lvuskovic",  "lvuskovic@tvz.hr",  "Lvuskovic123!"),
                ("lmodric",    "lmodric@tvz.hr",    "Lmodric123!"),
                ("mpasalic",   "mpasalic@tvz.hr",   "Mpasalic123!"),
                ("mbaturina",  "mbaturina@tvz.hr",  "Mbaturina123!"),
                ("psucic",     "psucic@tvz.hr",     "Psucic123!"),
                ("iperisic",   "iperisic@tvz.hr",   "Iperisic123!"),
                ("pmusa",      "pmusa@tvz.hr",      "Pmusa123!"),
            };

            foreach (var (username, email, password) in users)
            {
                var user = new AppUser { UserName = username, Email = email };
                await _userManager.CreateAsync(user, password);
                await _userManager.AddToRoleAsync(user, "User");
            }
        }

        private async Task SeedPlayingFieldsAsync()
        {
            var seedImagesDir = Path.Combine(_env.ContentRootPath, "SeedData", "Images");
            var wwwrootDir = Path.Combine(_env.WebRootPath, "images", "stadiums");
            Directory.CreateDirectory(wwwrootDir);

            var fields = new[]
            {
                (Field: new PlayingField { Name = "Sportski Centar Dvojka",            Description = "U prostranoj dvorani na Zagrebačkom velesajmu zaigrajte mali nogomet na umjetnoj travi najnovije generacije i zaboravite na stres i ostale poslovne obaveze.",                    ContactNumber = "+385 91 3243 100", IsOutdoor = false, Latitude = 45.7804, Longitude = 15.9667, Status = FieldStatus.Verified, SurfaceType = SurfaceType.ArtificialGrass }, ImageFile: "ScDvojka.jpg"    ),
                (Field: new PlayingField { Name = "NC Šalata (Paviljon 1 - Velesajam)", Description = "Nogometni centar Šalata se nalazi na zagrebačkom Velesajmu – paviljon broj 1. Centar je zamišljen kao mjesto druženja i sportskog nadmetanja poslovnih ljudi.",              ContactNumber = "098 276 444",      IsOutdoor = false, Latitude = 45.7789, Longitude = 15.9669, Status = FieldStatus.Verified, SurfaceType = SurfaceType.ArtificialGrass }, ImageFile: "NcSalata.jpg"    ),
                (Field: new PlayingField { Name = "Nogometni centar Stenjevec",        Description = "Mali nogomet se igra u najsuvremenijoj zračnoj dvorani.",                                                                                                                       ContactNumber = "+385 99 261 8818", IsOutdoor = false, Latitude = 45.8102, Longitude = 15.8844, Status = FieldStatus.Verified, SurfaceType = SurfaceType.ArtificialGrass }, ImageFile: "NcStenjevec.jpg" ),
                (Field: new PlayingField { Name = "Sportski centar Oranice",           Description = "Sportska rekreacija za sve generacije -> Tenis i mali nogomet!",                                                                                                               ContactNumber = "01 3883 733",      IsOutdoor = true,  Latitude = 45.8031, Longitude = 15.9083, Status = FieldStatus.Verified, SurfaceType = SurfaceType.ArtificialGrass }, ImageFile: "ScOranice.jpg"   ),
                (Field: new PlayingField { Name = "SC Concordia",                      Description = "SPORTSKI CENTAR CONCORDIA je centar s dugogodišnjom tradicijom djelovanja.",                                                                                                   ContactNumber = "013667420",        IsOutdoor = true,  Latitude = 45.8074, Longitude = 15.9360, Status = FieldStatus.Verified, SurfaceType = SurfaceType.ArtificialGrass }, ImageFile: "ScConcordia.jpg" ),
                (Field: new PlayingField { Name = "SC Klaka",                          Description = "Rezerviraj termin i osiguraj svojoj ekipi mjesto za igru u vrhunskim uvjetima.",                                                                                               ContactNumber = "099 / 466 44 44",  IsOutdoor = false, Latitude = 45.8400, Longitude = 16.0556, Status = FieldStatus.Verified, SurfaceType = SurfaceType.ArtificialGrass }, ImageFile: "ScKlaka.jpg"     ),
                (Field: new PlayingField { Name = "SC Savica",                         Description = "Pogodan je za provođenje velikih sportskih manifestacija i najam za nogometne utakmice.",                                                                                       ContactNumber = "+385 91 4040250",  IsOutdoor = true,  Latitude = 45.7880, Longitude = 16.0024, Status = FieldStatus.Verified, SurfaceType = SurfaceType.NaturalGrass    }, ImageFile: "SCSavica.jpg"    ),
                (Field: new PlayingField { Name = "NK Maksimir - Pomoćni tereni",      Description = "Najam za vaše nogometne termine na istoku Zagreba!",                                                                                                                           ContactNumber = "091/3450-226",     IsOutdoor = true,  Latitude = 45.8283, Longitude = 16.0133, Status = FieldStatus.Verified, SurfaceType = SurfaceType.NaturalGrass    }, ImageFile: "NCMaksimir.jpg"  ),
            };

            foreach (var (field, imageFile) in fields)
            {
                var result = await _stadiumStore.CreatePlayingField(field);
                if (!result.IsSuccess) continue;

                var fieldId = result.Value;
                var storedName = $"{Guid.NewGuid()}.jpg";
                var src = Path.Combine(seedImagesDir, imageFile);
                var dst = Path.Combine(wwwrootDir, storedName);

                if (File.Exists(src)) File.Copy(src, dst);

                await _stadiumStore.AddImageResourceAsync(new ImageResource
                {
                    Id = Guid.NewGuid(),
                    Path = $"/images/stadiums/{storedName}",
                    FileName = imageFile,
                    SizeBytes = File.Exists(src) ? new FileInfo(src).Length : 0,
                    ContentType = imageFile.EndsWith(".png") ? "image/png" : "image/jpeg",
                    UploadDate = DateTime.UtcNow,
                    PlayingFieldId = fieldId,
                });
            }
        }

        private async Task SeedPlayersAsync()
        {
            var usernames = new[]
            {
                ("dlivakovic", Position.Goalkeeper,  new DateOnly(1995, 9,  9),  "Profesionalni vratar, igram u klupskoj ligi."),
                ("jstanisic",  Position.Defender,    new DateOnly(2000, 4,  19), "Desni bek, volim se uključiti u napad."),
                ("jsutalo",    Position.Defender,    new DateOnly(2002, 11, 5),  "Centralni branič, jak u duelima."),
                ("jgvardiol",  Position.Defender,    new DateOnly(2002, 1,  23), "Lijevokrilni bek, brz i tehnički."),
                ("lvuskovic",  Position.Midfielder,  new DateOnly(2001, 5,  1),  "Vezni igrač, dobar u presingiranju."),
                ("lmodric",    Position.Midfielder,  new DateOnly(1985, 9,  9),  "Kontroliram sredinu terena već godinama."),
                ("mpasalic",   Position.Midfielder,  new DateOnly(1995, 2,  9),  "Napadački vezni, gol je moj cilj."),
                ("mbaturina",  Position.Midfielder,  new DateOnly(2004, 10, 18), "Mladi talent, energičan u veznom redu."),
                ("psucic",     Position.Midfielder,  new DateOnly(2003, 3,  11), "Kreativan veznjak, dobra ljevica."),
                ("iperisic",   Position.Forward,     new DateOnly(1989, 2,  2),  "Krilo, brzina i dribling su moje oružje."),
                ("pmusa",      Position.Forward,     new DateOnly(2001, 12, 28), "Napadač, uvijek tražim prostor iza obrane."),
            };

            foreach (var (username, position, dob, bio) in usernames)
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null) continue;

                await _playerStore.CreatePlayer(new Player
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Bio = bio,
                    PreferredPosition = position,
                    DateOfBirth = dob,
                });
            }
        }

        private async Task SeedPartiesAsync()
        {
            var players = await _playerStore.GetAllPlayersAsync();
            if (players.Count < 4) return;

            var fields = await _stadiumStore.GetAllStadiumsAsync();
            if (fields.Count == 0) return;

            var today = DateTime.UtcNow.Date;
            var rng = new Random(42);

            DateTime RandPastDate(int daysBack) =>
                today.AddDays(-daysBack).AddHours(rng.Next(18, 22));

            DateTime RandFutureDate() =>
                today.AddDays(rng.Next(3, 14)).AddHours(rng.Next(8, 22));

            var zagrebLocations = new[]
            {
                "Maksimir", "Trešnjevka", "Črnomerec", "Stenjevec", "Dubrava",
                "Sesvete", "Novi Zagreb", "Trnje", "Gornji Grad", "Remete",
            };

            string RandLocations(int seed)
            {
                var r = new Random(seed);
                return string.Join(", ", zagrebLocations.OrderBy(_ => r.Next()).Take(r.Next(2, 4)));
            }

            // 8 parties: leader index, member indexes, max members, days ago created, description, field index
            var partyDefs = new[]
            {
                new { LeaderIdx = 0,  MemberIdxs = new[] { 0, 1, 2, 3, 4  }, MaxMembers = 5,  DaysBack = 10, Desc = "Opuštena ekipa, igramo radi rekreacije i dobrog raspoloženja. Svi pozicionirani dobrodošli.",                              FieldIdx = 0 },
                new { LeaderIdx = 2,  MemberIdxs = new[] { 2, 5, 6, 7, 8  }, MaxMembers = 8,  DaysBack = 8,  Desc = "Ekipa prijatelja koja se druži kroz mali nogomet svaki tjedan. Dobra atmosfera zagarantirana.",                           FieldIdx = 1 },
                new { LeaderIdx = 4,  MemberIdxs = new[] { 4, 0, 9, 10, 1 }, MaxMembers = 10, DaysBack = 6,  Desc = "Kompetitivna skupina igrača koja želi ozbiljnije trenirati i natjecati se na turnirima.",                                FieldIdx = 2 },
                new { LeaderIdx = 7,  MemberIdxs = new[] { 7, 3, 5, 8, 10 }, MaxMembers = 5,  DaysBack = 14, Desc = "Tražimo motivirane igrače za redovite utakmice, nema ozbiljnih zahtjeva – samo ljubav prema igri!",                     FieldIdx = 3 },
                new { LeaderIdx = 1,  MemberIdxs = new[] { 1, 6, 9, 0, 2  }, MaxMembers = 6,  DaysBack = 12, Desc = "Juniori i veterani dobrodošli – igramo svaki petak navečer, bez pritiska i bez izgovora.",                              FieldIdx = 4 },
                new { LeaderIdx = 5,  MemberIdxs = new[] { 5, 3, 10, 7, 4 }, MaxMembers = 6,  DaysBack = 4,  Desc = "Tražimo tehnički potkovane igrače koji vole kombinatornu igru. Brzina nije uvjet – pametnjakovići dobrodošli.",          FieldIdx = 5 },
                new { LeaderIdx = 9,  MemberIdxs = new[] { 9, 8, 1, 6, 3  }, MaxMembers = 8,  DaysBack = 7,  Desc = "Grupa koja igra vikendom ujutro. Svježi zrak, dobra ekipa i kava poslije utakmice – to je naš recept za savršenu subotu.", FieldIdx = 6 },
                new { LeaderIdx = 10, MemberIdxs = new[] { 10, 2, 4, 6, 8 }, MaxMembers = 6,  DaysBack = 3,  Desc = "Igramo za zabavu, ali s ambicijom. Ako si spreman dati sve na terenu i otići na pivo poslije – ti si naš čovjek.",        FieldIdx = 7 },
            };

            foreach (var def in partyDefs)
            {
                var leader = players[def.LeaderIdx % players.Count];
                var matchDate = RandPastDate(def.DaysBack - 1);

                var partyResult = await _partyStore.CreateParty(new Party
                {
                    Id = Guid.NewGuid(),
                    PlayerCreatedId = leader.Id,
                    DateCreated = today.AddDays(-def.DaysBack),
                    MaxMembers = def.MaxMembers,
                    PartyDescription = def.Desc,
                    PreferredLocations = RandLocations(def.LeaderIdx * 7 + def.DaysBack),
                });

                if (!partyResult.IsSuccess) continue;

                var partyId = partyResult.Value;
                var memberPlayers = def.MemberIdxs
                    .Select(i => players[i % players.Count])
                    .DistinctBy(p => p.Id)
                    .ToList();

                await _partyStore.SyncMembersAsync(partyId, memberPlayers.Select(p => p.Id).ToList());

                // preferred playing dates (1 future, 1 past around match day)
                _dbContext.PreferredPlayingDates.Add(new PreferredPlayingDate
                {
                    Id = Guid.NewGuid(),
                    PartyId = partyId,
                    Date = matchDate,
                });
                _dbContext.PreferredPlayingDates.Add(new PreferredPlayingDate
                {
                    Id = Guid.NewGuid(),
                    PartyId = partyId,
                    Date = RandFutureDate(),
                });

                await _dbContext.SaveChangesAsync();

                var field = fields[def.FieldIdx % fields.Count];

                // scheduled match
                var scheduledMatch = new ScheduledMatch
                {
                    Id = Guid.NewGuid(),
                    PartyId = partyId,
                    PlayingFieldId = field.Id,
                    MatchDate = matchDate,
                };
                _dbContext.ScheduledMatches.Add(scheduledMatch);

                // attendances — all members attending
                foreach (var p in memberPlayers)
                {
                    _dbContext.ScheduledMatchAttendances.Add(new ScheduledMatchAttendance
                    {
                        Id = Guid.NewGuid(),
                        ScheduledMatchId = scheduledMatch.Id,
                        PlayerId = p.Id,
                        IsAttending = true,
                    });
                }

                await _dbContext.SaveChangesAsync();

                // split members into two teams
                var teamA = memberPlayers.Take(memberPlayers.Count / 2 + memberPlayers.Count % 2).ToList();
                var teamB = memberPlayers.Skip(teamA.Count).ToList();

                // randomise per-player goals/assists
                var r = new Random(def.LeaderIdx + def.DaysBack);
                int RandGoals()   => r.Next(0, 4);
                int RandAssists() => r.Next(0, 3);

                var goalsA = teamA.Select(_ => RandGoals()).ToList();
                var goalsB = teamB.Select(_ => RandGoals()).ToList();

                int scoreA = goalsA.Sum();
                int scoreB = goalsB.Sum();

                // match record
                var matchRecord = new MatchRecord
                {
                    Id = Guid.NewGuid(),
                    WasMatchHeld = true,
                    MatchHeld = matchDate,
                    PlayingFieldId = field.Id,
                    GoalsTeamA = scoreA,
                    GoalsTeamB = scoreB,
                    ScheduledMatchId = scheduledMatch.Id,
                };
                _dbContext.MatchRecords.Add(matchRecord);
                await _dbContext.SaveChangesAsync();

                // match players
                var matchPlayers = new List<MatchPlayer>();

                for (int i = 0; i < teamA.Count; i++)
                {
                    var mp = new MatchPlayer
                    {
                        Id = Guid.NewGuid(),
                        PlayerId = teamA[i].Id,
                        MatchRecordId = matchRecord.Id,
                        Team = Team.A,
                        Goals = goalsA[i],
                        Assists = RandAssists(),
                    };
                    _dbContext.MatchPlayers.Add(mp);
                    matchPlayers.Add(mp);
                }

                for (int i = 0; i < teamB.Count; i++)
                {
                    var mp = new MatchPlayer
                    {
                        Id = Guid.NewGuid(),
                        PlayerId = teamB[i].Id,
                        MatchRecordId = matchRecord.Id,
                        Team = Team.B,
                        Goals = goalsB[i],
                        Assists = RandAssists(),
                    };
                    _dbContext.MatchPlayers.Add(mp);
                    matchPlayers.Add(mp);
                }

                await _dbContext.SaveChangesAsync();

                // player ratings — each match player rates the next one in the list (round-robin)
                for (int i = 0; i < matchPlayers.Count; i++)
                {
                    var giver   = matchPlayers[i];
                    var receiver = matchPlayers[(i + 1) % matchPlayers.Count];
                    _dbContext.PlayerRatings.Add(new PlayerRating
                    {
                        Id = Guid.NewGuid(),
                        MatchPlayerId = giver.Id,
                        PlayerGivingRatingId = giver.PlayerId,
                        PlayerReceivingRatingId = receiver.PlayerId,
                        Rating = r.Next(5, 10),
                    });
                }

                // match votes — everyone votes the match was held
                foreach (var p in memberPlayers)
                {
                    _dbContext.MatchVotes.Add(new MatchVote
                    {
                        Id = Guid.NewGuid(),
                        MatchRecordId = matchRecord.Id,
                        PlayerId = p.Id,
                        VotedHeld = true,
                    });
                }

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
