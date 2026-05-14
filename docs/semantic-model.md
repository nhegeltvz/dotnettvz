# Semantic model

This is a semantic model of my application + How app was imagined to work

## App idea

The original idea of this web application is a user based experience where users find recreation football matches in Zagreb.
Workflow of using the app is:
User logs in and has a username
=> He is able to create a party with x number of members allowed.
=> He is able to join a party if the number of spots is still under the limit.

After a party is created and has some members:
=> Although we will not currently implement chat section, that would be ideal use case of party entity, to be able to exchange messages.
=> In a party people discuss time and place for their match and finally they select time and one of the already created playing fields entities.
=> They confirm their Arrival at the event any time before the event actually started (-1 hour maybe)

After the match is over:
=> Match Players are asked wether the event was held or not to keep the integrity of the data in database
=> If the match was taken, players randomly rate each other through "PlayerRating" entity.

Playing field:
=> currently not configured perfectly, but the idea is that users will be able to add playing field themselves

Every other data displayed in the application is derived from data extracted through this work flow.

## Entity relationships (technical model)

This section documents how the current EF Core entities relate to each other.

### Player

- One Player creates many Parties: `Player.CreatedParties` -> `Party.PlayerCreatedId`.
- Many Players join many Parties: `Player.JoinedParties` <-> `Party.Members` (implicit many-to-many).
- One Player participates in many MatchPlayer rows: `Player.MatchPlayers` -> `MatchPlayer.PlayerId`.
- One Player can cast many MatchVote rows: `Player.MatchVotes` -> `MatchVote.PlayerId`.
- One Player can give many PlayerRating rows: `Player.RatingsGiven` -> `PlayerRating.PlayerGivingRatingId`.
- One Player can receive many PlayerRating rows: `Player.RatingsReceived` -> `PlayerRating.PlayerReceivingRatingId`.
- One Player can have many ScheduledMatchAttendance rows: `Player.ScheduledMatchAttendances` -> `ScheduledMatchAttendance.PlayerId`.

Delete behavior:

- Deleting a Player is blocked if they created any Party (`Restrict`).
- Deleting a Player is blocked if they are linked to MatchPlayer rows (`Restrict`).
- Deleting a Player is blocked if they cast MatchVote rows (`Restrict`).
- Deleting a Player is blocked if they gave or received PlayerRating rows (`Restrict`).
- Deleting a Player is blocked if they have ScheduledMatchAttendance rows (`Restrict`).
- Deleting a Player removes their join-table rows in `PartyMembers` (many-to-many default).

### Party

- Party belongs to one creator: `Party.PlayerCreatedId` -> `Player`.
- Party has many members: `Party.Members` <-> `Player.JoinedParties`.
- Party has many PreferredPlayingDate rows: `Party.PreferredPlayingDates` -> `PreferredPlayingDate.PartyId`.
- Party can have zero or one ScheduledMatch: `Party.ScheduledMatch` -> `ScheduledMatch.PartyId`.

Delete behavior:

- Deleting a Party deletes all PreferredPlayingDate rows for that party (`Cascade`).
- Deleting a Party deletes its ScheduledMatch if one exists (`Cascade`).
- Deleting a Party removes its join-table rows in `PartyMembers` (many-to-many default).

### PreferredPlayingDate

- Each PreferredPlayingDate belongs to one Party: `PreferredPlayingDate.PartyId` -> `Party`.

Delete behavior:

- Deleting a Party deletes all its PreferredPlayingDate rows (`Cascade`).

### ScheduledMatch

- ScheduledMatch belongs to one Party: `ScheduledMatch.PartyId` -> `Party`.
- ScheduledMatch belongs to one PlayingField: `ScheduledMatch.PlayingFieldId` -> `PlayingField`.
- ScheduledMatch has many ScheduledMatchAttendance rows: `ScheduledMatch.ScheduledMatchAttendances` -> `ScheduledMatchAttendance.ScheduledMatchId`.

Delete behavior:

- Deleting a Party deletes its ScheduledMatch (`Cascade`).
- Deleting a ScheduledMatch deletes all ScheduledMatchAttendance rows (`Cascade`).
- Deleting a PlayingField is blocked if any ScheduledMatch points to it (`Restrict`).

### ScheduledMatchAttendance

- Each attendance belongs to one ScheduledMatch: `ScheduledMatchAttendance.ScheduledMatchId` -> `ScheduledMatch`.
- Each attendance belongs to one Player: `ScheduledMatchAttendance.PlayerId` -> `Player`.

Delete behavior:

- Deleting a ScheduledMatch deletes all its attendance rows (`Cascade`).
- Deleting a Player is blocked if they have attendance rows (`Restrict`).

### PlayingField

- PlayingField has many MatchRecord rows: `PlayingField.MatchRecords` -> `MatchRecord.PlayingFieldId`.
- PlayingField has many ScheduledMatch rows: `PlayingField.ScheduledMatches` -> `ScheduledMatch.PlayingFieldId`.

Delete behavior:

- Deleting a PlayingField is blocked if any MatchRecord points to it (`Restrict`).
- Deleting a PlayingField is blocked if any ScheduledMatch points to it (`Restrict`).

### MatchRecord

- MatchRecord belongs to one PlayingField: `MatchRecord.PlayingFieldId` -> `PlayingField`.
- MatchRecord has many MatchPlayer rows: `MatchRecord.MatchPlayers` -> `MatchPlayer.MatchRecordId`.
- MatchRecord has many MatchVote rows: `MatchRecord.MatchVotes` -> `MatchVote.MatchRecordId`.

Delete behavior:

- Deleting a MatchRecord deletes all MatchPlayer rows (`Cascade`).
- Deleting a MatchRecord deletes all MatchVote rows (`Cascade`).
- Deleting a PlayingField is blocked if any MatchRecord points to it (`Restrict`).

### MatchPlayer

- MatchPlayer belongs to one Player: `MatchPlayer.PlayerId` -> `Player`.
- MatchPlayer belongs to one MatchRecord: `MatchPlayer.MatchRecordId` -> `MatchRecord`.
- MatchPlayer can have zero or one PlayerRating: `MatchPlayer.PlayerRating` -> `PlayerRating.MatchPlayerId`.

Delete behavior:

- Deleting a MatchRecord deletes its MatchPlayer rows (`Cascade`).
- Deleting a MatchPlayer deletes its PlayerRating row if one exists (`Cascade`).
- Deleting a Player is blocked if they are linked via MatchPlayer (`Restrict`).

### MatchVote

- MatchVote belongs to one MatchRecord: `MatchVote.MatchRecordId` -> `MatchRecord`.
- MatchVote belongs to one Player: `MatchVote.PlayerId` -> `Player`.

Delete behavior:

- Deleting a MatchRecord deletes its MatchVote rows (`Cascade`).
- Deleting a Player is blocked if they are linked via MatchVote (`Restrict`).

### PlayerRating

- PlayerRating belongs to one MatchPlayer: `PlayerRating.MatchPlayerId` -> `MatchPlayer`.
- PlayerRating has one giver Player: `PlayerRating.PlayerGivingRatingId` -> `Player`.
- PlayerRating has one receiver Player: `PlayerRating.PlayerReceivingRatingId` -> `Player`.

Delete behavior:

- Deleting a MatchPlayer deletes its PlayerRating row (`Cascade`).
- Deleting a Player is blocked if they gave or received ratings (`Restrict`).
