using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Data;

public class MatchTrackerDbContext : DbContext
{
    public MatchTrackerDbContext(DbContextOptions<MatchTrackerDbContext> options) : base(options) { }

    public DbSet<MatchPlayer> MatchPlayers { get; set; } = null!;
    public DbSet<MatchRecord> MatchRecords { get; set; } = null!;
    public DbSet<MatchVote> MatchVotes { get; set; } = null!;
    public DbSet<Party> Parties { get; set; } = null!;
    public DbSet<Player> Players { get; set; } = null!;
    public DbSet<PlayerRating> PlayerRatings { get; set; } = null!;
    public DbSet<PlayingField> PlayingFields { get; set; } = null!;
    public DbSet<PreferredPlayingDate> PreferredPlayingDates { get; set; } = null!;
    public DbSet<ScheduledMatch> ScheduledMatches { get; set; } = null!;
    public DbSet<ScheduledMatchAttendance> ScheduledMatchAttendances { get; set; } = null!;
    public DbSet<ImageResource> Images { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Party>()
            .HasOne(party => party.PlayerCreated)
            .WithMany(player => player.CreatedParties)
            .HasForeignKey(party => party.PlayerCreatedId)
            //Restrict deleting a player that is associated with party
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Party>()
            .HasMany(party => party.Members)
            .WithMany(player => player.JoinedParties)
            .UsingEntity(join => join.ToTable("PartyMembers"));

        modelBuilder.Entity<MatchRecord>()
            .HasOne(record => record.PlayingField)
            .WithMany(field => field.MatchRecords)
            .HasForeignKey(record => record.PlayingFieldId)
            //Don't delete the Playing field if any match record is associated with it.
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MatchPlayer>()
            .HasOne(matchPlayer => matchPlayer.MatchRecord)
            .WithMany(matchRecord => matchRecord.MatchPlayers)
            .HasForeignKey(matchPlayer => matchPlayer.MatchRecordId)
            //when Match record is deleted, delete all associated MatchPlayers
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MatchPlayer>()
            .HasOne(matchPlayer => matchPlayer.Player)
            .WithMany(player => player.MatchPlayers)
            .HasForeignKey(matchPlayer => matchPlayer.PlayerId)
            //Restrict deleting a player that is connected to a match record through MatchPlayer
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MatchVote>()
            .HasOne(matchVote => matchVote.MatchRecord)
            .WithMany(matchRecord => matchRecord.MatchVotes)
            .HasForeignKey(matchVote => matchVote.MatchRecordId)
            //When Match record is deleted, delete all associated MatchVotes
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MatchVote>()
            .HasOne(matchVote => matchVote.Player)
            .WithMany(player => player.MatchVotes)
            .HasForeignKey(matchVote => matchVote.PlayerId)
            //Restrict Deleting a player that gave a vote in MatchVote
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PlayerRating>()
            .HasOne(rating => rating.MatchPlayer)
            .WithOne(matchPlayer => matchPlayer.PlayerRating)
            .HasForeignKey<PlayerRating>(rating => rating.MatchPlayerId)
            //After Deleting a Match Player, delete the associated PlayerRating
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PlayerRating>()
            .HasOne(rating => rating.PlayerGivingRating)
            .WithMany(player => player.RatingsGiven)
            .HasForeignKey(rating => rating.PlayerGivingRatingId)
            //Rectrict deletion of player giving the rating if any rating was given by that player
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PlayerRating>()
            .HasOne(rating => rating.PlayerReceivingRating)
            .WithMany(player => player.RatingsReceived)
            .HasForeignKey(rating => rating.PlayerReceivingRatingId)
            //Rectrict deletion of player receiving the rating if any rating was received by that player
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PreferredPlayingDate>()
            .HasOne(preferredDate => preferredDate.Party)
            .WithMany(party => party.PreferredPlayingDates)
            .HasForeignKey(preferredDate => preferredDate.PartyId)
            //If a party is deleted, delete all associated preferred playing dates
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ScheduledMatch>()
            .HasOne(scheduledMatch => scheduledMatch.PlayingField)
            .WithMany(field => field.ScheduledMatches)
            .HasForeignKey(scheduledMatch => scheduledMatch.PlayingFieldId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ScheduledMatch>()
            .HasOne(ScheduledMatch => ScheduledMatch.Party)
            .WithOne(party => party.ScheduledMatch)
            .HasForeignKey<ScheduledMatch>(scheduledMatch => scheduledMatch.PartyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ScheduledMatchAttendance>()
            .HasOne(attendance => attendance.ScheduledMatch)
            .WithMany(scheduledMatch => scheduledMatch.ScheduledMatchAttendances)
            .HasForeignKey(attendance => attendance.ScheduledMatchId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<ScheduledMatchAttendance>()
            .HasOne(attendance => attendance.Player)
            .WithMany(player => player.ScheduledMatchAttendances)
            .HasForeignKey(attendance => attendance.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ImageResource>()
            .HasOne(img => img.PlayingField)
            .WithMany(pf => pf.Images)
            .HasForeignKey(img => img.PlayingFieldId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
