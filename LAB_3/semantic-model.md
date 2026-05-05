# Semantic model

This file summarizes the core entities/tables and how they relate in the Data project.

## Entities and key fields

- Player
  - Id, Username, Email, Bio, PreferredPosition, Age, ProfilePicture
- Party
  - Id, PlayerCreatedId, DateCreated, MaxMembers, PartyDescription, PreferredLocations
- PreferredPlayingDate
  - Id, PartyId, Date
- PlayingField
  - Id, Name, Description, Longitude, Latitude, Image, ContactNumber, Status, IsOutdoor, SurfaceType, CountOfPlayedMatches
- MatchRecord
  - Id, WasMatchHeld, MatchHeld, PlayingFieldId, GoalsTeamA, GoalsTeamB
- MatchPlayer
  - Id, PlayerId, MatchRecordId, Team, Goals, Assists
- MatchVote
  - Id, MatchRecordId, PlayerId, VotedHeld
- PlayerRating
  - Id, MatchPlayerId, PlayerGivingRatingId, PlayerReceivingRatingId, Rating

## Relationships

- Player (1) -> Party (many) as organizer via Party.PlayerCreatedId
- Player (many) <-> Party (many) via join table PartyMembers
- Party (1) -> PreferredPlayingDate (many) via PreferredPlayingDate.PartyId
- PlayingField (1) -> MatchRecord (many) via MatchRecord.PlayingFieldId
- MatchRecord (1) -> MatchPlayer (many) via MatchPlayer.MatchRecordId
- MatchRecord (1) -> MatchVote (many) via MatchVote.MatchRecordId
- Player (1) -> MatchPlayer (many) via MatchPlayer.PlayerId
- Player (1) -> MatchVote (many) via MatchVote.PlayerId
- MatchPlayer (1) -> PlayerRating (1) via PlayerRating.MatchPlayerId
- Player (1) -> PlayerRating (many) as giver via PlayerRating.PlayerGivingRatingId
- Player (1) -> PlayerRating (many) as receiver via PlayerRating.PlayerReceivingRatingId

## Enum fields

- Player.PreferredPosition: Position (Goalkeeper, Defender, Midfielder, Forward)
- MatchPlayer.Team: Team (A, B)
- PlayingField.Status: FieldStatus (Verified, PendingApproval, UnderMaintenance)
- PlayingField.SurfaceType: SurfaceType (NaturalGrass, ArtificialGrass, Concrete, Indoor)
