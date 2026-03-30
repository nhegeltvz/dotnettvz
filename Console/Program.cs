using Data.Data;
using Data.Models;
using System.Runtime.CompilerServices;

// -------------------------
// PLAYERS
// -------------------------

var player1 = new Player { Id = Guid.NewGuid(), Username = "Nitkov",   Email = "nitkov@tvz.hr",   Age = 22, Bio = "Ubojiti strijelac sa dvije lijeve",          PreferredPosition = Position.Forward     };
var player2 = new Player { Id = Guid.NewGuid(), Username = "Kovacic",  Email = "kovacic@tvz.hr",  Age = 25, Bio = "Kontroliram srediste terena vec 10 godina",   PreferredPosition = Position.Midfielder  };
var player3 = new Player { Id = Guid.NewGuid(), Username = "Zubak",    Email = "zubak@tvz.hr",    Age = 30, Bio = "Cuvam gol kao lavica mladunce",               PreferredPosition = Position.Goalkeeper  };
var player4 = new Player { Id = Guid.NewGuid(), Username = "Maric",    Email = "maric@tvz.hr",    Age = 28, Bio = "Nitko ne prolazi kroz moju obranu",           PreferredPosition = Position.Defender    };

var players = new List<Player> { player1, player2, player3, player4 };

// -------------------------
// PLAYING FIELDS
// -------------------------

var field1 = new PlayingField { Id = Guid.NewGuid(), Name = "SC Dvojka",         Description = "Teren sa umjetnom travom i kafanom ispred.", ContactNumber = "095 888 6666", CountOfPlayedMatches = 0, IsOutdoor = false, Latitude = 45.7802, Longitude = 15.9655, Status = FieldStatus.Verified,         SurfaceType = SurfaceType.ArtificialGrass };
var field2 = new PlayingField { Id = Guid.NewGuid(), Name = "Maksimir Park",      Description = "Otvoreni teren uz park, prirodna trava.",      ContactNumber = "091 234 5678", CountOfPlayedMatches = 0, IsOutdoor = true,  Latitude = 45.8144, Longitude = 16.0197, Status = FieldStatus.Verified,         SurfaceType = SurfaceType.NaturalGrass    };
var field3 = new PlayingField { Id = Guid.NewGuid(), Name = "Asfalt Tresnjevka",  Description = "Betonski teren u kvartu, ceka odobrenje.",      ContactNumber = "098 765 4321", CountOfPlayedMatches = 0, IsOutdoor = true,  Latitude = 45.7985, Longitude = 15.9412, Status = FieldStatus.PendingApproval, SurfaceType = SurfaceType.Concrete        };

var fields = new List<PlayingField> { field1, field2, field3 };

// -------------------------
// PARTIES
// -------------------------

var party1 = new Party { Id = Guid.NewGuid(), DateCreated = DateTime.Now.AddDays(-40), MaxMembers = 10, PartyDescription = "Trazim pocetnike za rekreacijski nogomet", PlayerCreatedId = player1.Id, PreferredLocations = "Istok Zagreba"        };
var party2 = new Party { Id = Guid.NewGuid(), DateCreated = DateTime.Now.AddDays(-7),  MaxMembers = 6,  PartyDescription = "Iskusni igraci, igramo svaki tjedan",      PlayerCreatedId = player2.Id, PreferredLocations = "Maksimir ili Jarun"   };
var party3 = new Party { Id = Guid.NewGuid(), DateCreated = DateTime.Now.AddDays(-3),  MaxMembers = 8,  PartyDescription = "Vikend ekipa, subotom ujutro",              PlayerCreatedId = player3.Id, PreferredLocations = "Tresnjevka, Crnomerec" };

party1.Members.AddRange(new[] { player1, player2 });
party2.Members.AddRange(new[] { player2, player3 });
party3.Members.AddRange(new[] { player3, player4 });

var parties = new List<Party> { party1, party2, party3 };

// -------------------------
// PREFERRED PLAYING DATES
// -------------------------

var date1 = new PreferredPlayingDate { Id = Guid.NewGuid(), PartyId = party1.Id, Date = DateTime.Now.AddDays(3) };
var date2 = new PreferredPlayingDate { Id = Guid.NewGuid(), PartyId = party2.Id, Date = DateTime.Now.AddDays(7) };
var date3 = new PreferredPlayingDate { Id = Guid.NewGuid(), PartyId = party3.Id, Date = DateTime.Now.AddDays(2) };

var preferredDates = new List<PreferredPlayingDate> { date1, date2, date3 };

// -------------------------
// MATCH RECORDS
// -------------------------

var match1 = new MatchRecord { Id = Guid.NewGuid(), MatchHeld = DateTime.Now.AddDays(-15), PlayingFieldId = field1.Id, WasMatchHeld = true,  GoalsTeamA = 3, GoalsTeamB = 2 };
var match2 = new MatchRecord { Id = Guid.NewGuid(), MatchHeld = DateTime.Now.AddDays(-2), PlayingFieldId = field2.Id, WasMatchHeld = true,  GoalsTeamA = 1, GoalsTeamB = 1 };
var match3 = new MatchRecord { Id = Guid.NewGuid(), MatchHeld = DateTime.Now.AddDays(4),  PlayingFieldId = field3.Id, WasMatchHeld = false, GoalsTeamA = 0, GoalsTeamB = 0 };

var matches = new List<MatchRecord> { match1, match2, match3 };

// -------------------------
// MATCH PLAYERS
// -------------------------

var mp1 = new MatchPlayer { Id = Guid.NewGuid(), PlayerId = player1.Id, MatchRecordId = match1.Id, Team = Team.A, Goals = 2, Assists = 1 };
var mp2 = new MatchPlayer { Id = Guid.NewGuid(), PlayerId = player2.Id, MatchRecordId = match1.Id, Team = Team.B, Goals = 1, Assists = 0 };
var mp3 = new MatchPlayer { Id = Guid.NewGuid(), PlayerId = player2.Id, MatchRecordId = match2.Id, Team = Team.A, Goals = 1, Assists = 0 };
var mp4 = new MatchPlayer { Id = Guid.NewGuid(), PlayerId = player3.Id, MatchRecordId = match2.Id, Team = Team.B, Goals = 1, Assists = 1 };
var mp5 = new MatchPlayer { Id = Guid.NewGuid(), PlayerId = player3.Id, MatchRecordId = match3.Id, Team = Team.A, Goals = 0, Assists = 0 };
var mp6 = new MatchPlayer { Id = Guid.NewGuid(), PlayerId = player4.Id, MatchRecordId = match3.Id, Team = Team.B, Goals = 0, Assists = 0 };

var matchPlayers = new List<MatchPlayer> { mp1, mp2, mp3, mp4, mp5, mp6 };

// -------------------------
// PLAYER RATINGS (completed matches only)
// -------------------------

var rating1 = new PlayerRating { Id = Guid.NewGuid(), MatchPlayerId = mp1.Id, PlayerGivingRatingId = player2.Id, PlayerReceivingRatingId = player1.Id, Rating = 9 };
var rating2 = new PlayerRating { Id = Guid.NewGuid(), MatchPlayerId = mp3.Id, PlayerGivingRatingId = player3.Id, PlayerReceivingRatingId = player2.Id, Rating = 7 };
var rating3 = new PlayerRating { Id = Guid.NewGuid(), MatchPlayerId = mp4.Id, PlayerGivingRatingId = player2.Id, PlayerReceivingRatingId = player3.Id, Rating = 8 };

var ratings = new List<PlayerRating> { rating1, rating2, rating3 };

// -------------------------
// MATCH VOTES
// -------------------------

var vote1 = new MatchVote { Id = Guid.NewGuid(), MatchRecordId = match1.Id, PlayerId = player1.Id, VotedHeld = true  };
var vote2 = new MatchVote { Id = Guid.NewGuid(), MatchRecordId = match2.Id, PlayerId = player2.Id, VotedHeld = true  };
var vote3 = new MatchVote { Id = Guid.NewGuid(), MatchRecordId = match3.Id, PlayerId = player3.Id, VotedHeld = false };

var matchVotes = new List<MatchVote> { vote1, vote2, vote3 };

// -------------------------
// NAVIGATION PROPERTY WIRING
// (EF does this automatically from the database; in-memory we do it manually)
// -------------------------

// Party -> PlayerCreated, Player -> CreatedParties
party1.PlayerCreated = player1;  player1.CreatedParties.Add(party1);
party2.PlayerCreated = player2;  player2.CreatedParties.Add(party2);
party3.PlayerCreated = player3;  player3.CreatedParties.Add(party3);

// Player -> JoinedParties (inverse side of the N-N already wired via party.Members)
player1.JoinedParties.Add(party1);
player2.JoinedParties.AddRange(new[] { party1, party2 });
player3.JoinedParties.AddRange(new[] { party2, party3 });
player4.JoinedParties.Add(party3);

// PreferredPlayingDate -> Party, Party -> PreferredPlayingDates
date1.Party = party1;  party1.PreferredPlayingDates.Add(date1);
date2.Party = party2;  party2.PreferredPlayingDates.Add(date2);
date3.Party = party3;  party3.PreferredPlayingDates.Add(date3);

// MatchRecord -> PlayingField, PlayingField -> MatchRecords
match1.PlayingField = field1;  field1.MatchRecords.Add(match1);
match2.PlayingField = field2;  field2.MatchRecords.Add(match2);
match3.PlayingField = field3;  field3.MatchRecords.Add(match3);

// MatchPlayer -> Player, Player -> MatchPlayers
mp1.Player = player1;  player1.MatchPlayers.Add(mp1);
mp2.Player = player2;  player2.MatchPlayers.Add(mp2);
mp3.Player = player2;  player2.MatchPlayers.Add(mp3);
mp4.Player = player3;  player3.MatchPlayers.Add(mp4);
mp5.Player = player3;  player3.MatchPlayers.Add(mp5);
mp6.Player = player4;  player4.MatchPlayers.Add(mp6);

// MatchPlayer -> MatchRecord, MatchRecord -> MatchPlayers
mp1.MatchRecord = match1;  mp2.MatchRecord = match1;  match1.MatchPlayers.AddRange(new[] { mp1, mp2 });
mp3.MatchRecord = match2;  mp4.MatchRecord = match2;  match2.MatchPlayers.AddRange(new[] { mp3, mp4 });
mp5.MatchRecord = match3;  mp6.MatchRecord = match3;  match3.MatchPlayers.AddRange(new[] { mp5, mp6 });

// PlayerRating -> MatchPlayer, MatchPlayer -> PlayerRating
rating1.MatchPlayer = mp1;  mp1.PlayerRating = rating1;
rating2.MatchPlayer = mp3;  mp3.PlayerRating = rating2;
rating3.MatchPlayer = mp4;  mp4.PlayerRating = rating3;

// PlayerRating -> giving/receiving players, Player -> RatingsGiven/RatingsReceived
rating1.PlayerGivingRating = player2;  rating1.PlayerReceivingRating = player1;
rating2.PlayerGivingRating = player3;  rating2.PlayerReceivingRating = player2;
rating3.PlayerGivingRating = player2;  rating3.PlayerReceivingRating = player3;
player2.RatingsGiven.AddRange(new[] { rating1, rating3 });
player3.RatingsGiven.Add(rating2);
player1.RatingsReceived.Add(rating1);
player2.RatingsReceived.Add(rating2);
player3.RatingsReceived.Add(rating3);

// MatchVote -> MatchRecord/Player, MatchRecord -> MatchVotes, Player -> MatchVotes
vote1.MatchRecord = match1;  vote1.Player = player1;  match1.MatchVotes.Add(vote1);  player1.MatchVotes.Add(vote1);
vote2.MatchRecord = match2;  vote2.Player = player2;  match2.MatchVotes.Add(vote2);  player2.MatchVotes.Add(vote2);
vote3.MatchRecord = match3;  vote3.Player = player3;  match3.MatchVotes.Add(vote3);  player3.MatchVotes.Add(vote3);

// Find the player with most goals in the last month -> simulating finding player of the month.

Console.WriteLine(" --- Player of the month --- ");
var usernamePlayerOfMonth = matches.Where(m => m.WasMatchHeld && m.MatchHeld >= DateTime.Now.AddDays(-30)).SelectMany(m => m.MatchPlayers).MaxBy(mp => (mp.Goals + mp.Assists)).Player.Username;
Console.WriteLine(usernamePlayerOfMonth ?? "Dva igraca dijele jednaku statistiku golova i asistencija...");


// Find fields which are artifical grass and are indoor -> simulating in app filtering.
Console.WriteLine(" --- Filtered playing fields which are indoor and on artifical grass --- ");
fields.Where(f => !f.IsOutdoor && f.SurfaceType == SurfaceType.ArtificialGrass).Select(f => f.Name).ToList().ForEach(fieldName => Console.WriteLine(fieldName));

// Find players which haven't been a part of any matches -> simulating admin pushing notifications to inactive users.

var inactivePlayers = players.Where(p => !p.MatchPlayers.Any(mp => mp.MatchRecord.WasMatchHeld));

Console.WriteLine("--- Send notification to inactive players ---");
foreach (var p in inactivePlayers)
    Console.WriteLine(p.Username);
