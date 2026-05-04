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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Party>()
            .HasOne(party => party.PlayerCreated)
            .WithMany(player => player.CreatedParties)
            .HasForeignKey(party => party.PlayerCreatedId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Party>()
            .HasMany(party => party.Members)
            .WithMany(player => player.JoinedParties)
            .UsingEntity(join => join.ToTable("PartyMembers"));

        modelBuilder.Entity<MatchRecord>()
            .HasOne(record => record.PlayingField)
            .WithMany(field => field.MatchRecords)
            .HasForeignKey(record => record.PlayingFieldId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MatchPlayer>()
            .HasOne(matchPlayer => matchPlayer.MatchRecord)
            .WithMany(matchRecord => matchRecord.MatchPlayers)
            .HasForeignKey(matchPlayer => matchPlayer.MatchRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MatchPlayer>()
            .HasOne(matchPlayer => matchPlayer.Player)
            .WithMany(player => player.MatchPlayers)
            .HasForeignKey(matchPlayer => matchPlayer.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MatchVote>()
            .HasOne(matchVote => matchVote.MatchRecord)
            .WithMany(matchRecord => matchRecord.MatchVotes)
            .HasForeignKey(matchVote => matchVote.MatchRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MatchVote>()
            .HasOne(matchVote => matchVote.Player)
            .WithMany(player => player.MatchVotes)
            .HasForeignKey(matchVote => matchVote.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PlayerRating>()
            .HasOne(rating => rating.MatchPlayer)
            .WithOne(matchPlayer => matchPlayer.PlayerRating)
            .HasForeignKey<PlayerRating>(rating => rating.MatchPlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PlayerRating>()
            .HasOne(rating => rating.PlayerGivingRating)
            .WithMany(player => player.RatingsGiven)
            .HasForeignKey(rating => rating.PlayerGivingRatingId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PlayerRating>()
            .HasOne(rating => rating.PlayerReceivingRating)
            .WithMany(player => player.RatingsReceived)
            .HasForeignKey(rating => rating.PlayerReceivingRatingId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PreferredPlayingDate>()
            .HasOne(preferredDate => preferredDate.Party)
            .WithMany(party => party.PreferredPlayingDates)
            .HasForeignKey(preferredDate => preferredDate.PartyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
