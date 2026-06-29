using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Data;

public class MatchTrackerDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public MatchTrackerDbContext(DbContextOptions<MatchTrackerDbContext> options) : base(options) { }

    public DbSet<Player> Players { get; set; } = null!;
    public DbSet<MatchPlayer> MatchPlayers { get; set; } = null!;
    public DbSet<MatchRecord> MatchRecords { get; set; } = null!;
    public DbSet<MatchVote> MatchVotes { get; set; } = null!;
    public DbSet<Party> Parties { get; set; } = null!;
    public DbSet<PlayerRating> PlayerRatings { get; set; } = null!;
    public DbSet<PlayingField> PlayingFields { get; set; } = null!;
    public DbSet<PreferredPlayingDate> PreferredPlayingDates { get; set; } = null!;
    public DbSet<ScheduledMatch> ScheduledMatches { get; set; } = null!;
    public DbSet<ScheduledMatchAttendance> ScheduledMatchAttendances { get; set; } = null!;
    public DbSet<ImageResource> Images { get; set; } = null!;
    public DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // AppUser → Player (1-to-0..1)
        modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Player)
            .WithOne(p => p.User)
            .HasForeignKey<Player>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Party → PlayerCreated (Player) — delete the party when its creator is deleted
        modelBuilder.Entity<Party>()
            .HasOne(party => party.PlayerCreated)
            .WithMany(player => player.CreatedParties)
            .HasForeignKey(party => party.PlayerCreatedId)
            .OnDelete(DeleteBehavior.Cascade);

        // Party ↔ Members (many-to-many Player)
        modelBuilder.Entity<Party>()
            .HasMany(party => party.Members)
            .WithMany(player => player.JoinedParties)
            .UsingEntity(join => join.ToTable("PartyMembers"));

        // MatchRecord → ScheduledMatch (1-to-1, nullable)
        modelBuilder.Entity<MatchRecord>()
            .HasOne(mr => mr.ScheduledMatch)
            .WithOne(sm => sm.MatchRecord)
            .HasForeignKey<MatchRecord>(mr => mr.ScheduledMatchId)
            .OnDelete(DeleteBehavior.SetNull);

        // MatchRecord → PlayingField
        modelBuilder.Entity<MatchRecord>()
            .HasOne(record => record.PlayingField)
            .WithMany(field => field.MatchRecords)
            .HasForeignKey(record => record.PlayingFieldId)
            .OnDelete(DeleteBehavior.Restrict);

        // MatchPlayer → MatchRecord
        modelBuilder.Entity<MatchPlayer>()
            .HasOne(matchPlayer => matchPlayer.MatchRecord)
            .WithMany(matchRecord => matchRecord.MatchPlayers)
            .HasForeignKey(matchPlayer => matchPlayer.MatchRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        // MatchPlayer → Player — remove player entry from match records on delete
        modelBuilder.Entity<MatchPlayer>()
            .HasOne(matchPlayer => matchPlayer.Player)
            .WithMany(player => player.MatchPlayers)
            .HasForeignKey(matchPlayer => matchPlayer.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        // MatchVote → MatchRecord
        modelBuilder.Entity<MatchVote>()
            .HasOne(matchVote => matchVote.MatchRecord)
            .WithMany(matchRecord => matchRecord.MatchVotes)
            .HasForeignKey(matchVote => matchVote.MatchRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        // MatchVote → Player — remove votes on player delete
        modelBuilder.Entity<MatchVote>()
            .HasOne(matchVote => matchVote.Player)
            .WithMany(player => player.MatchVotes)
            .HasForeignKey(matchVote => matchVote.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        // PlayerRating → MatchPlayer
        modelBuilder.Entity<PlayerRating>()
            .HasOne(rating => rating.MatchPlayer)
            .WithOne(matchPlayer => matchPlayer.PlayerRating)
            .HasForeignKey<PlayerRating>(rating => rating.MatchPlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        // PlayerRating → PlayerGivingRating (Player)
        modelBuilder.Entity<PlayerRating>()
            .HasOne(rating => rating.PlayerGivingRating)
            .WithMany(player => player.RatingsGiven)
            .HasForeignKey(rating => rating.PlayerGivingRatingId)
            .OnDelete(DeleteBehavior.Cascade);

        // PlayerRating → PlayerReceivingRating (Player)
        modelBuilder.Entity<PlayerRating>()
            .HasOne(rating => rating.PlayerReceivingRating)
            .WithMany(player => player.RatingsReceived)
            .HasForeignKey(rating => rating.PlayerReceivingRatingId)
            .OnDelete(DeleteBehavior.Cascade);

        // PreferredPlayingDate → Party
        modelBuilder.Entity<PreferredPlayingDate>()
            .HasOne(preferredDate => preferredDate.Party)
            .WithMany(party => party.PreferredPlayingDates)
            .HasForeignKey(preferredDate => preferredDate.PartyId)
            .OnDelete(DeleteBehavior.Cascade);

        // ScheduledMatch → PlayingField
        modelBuilder.Entity<ScheduledMatch>()
            .HasOne(scheduledMatch => scheduledMatch.PlayingField)
            .WithMany(field => field.ScheduledMatches)
            .HasForeignKey(scheduledMatch => scheduledMatch.PlayingFieldId)
            .OnDelete(DeleteBehavior.Restrict);

        // ScheduledMatch → Party (1-to-1)
        modelBuilder.Entity<ScheduledMatch>()
            .HasOne(scheduledMatch => scheduledMatch.Party)
            .WithOne(party => party.ScheduledMatch)
            .HasForeignKey<ScheduledMatch>(scheduledMatch => scheduledMatch.PartyId)
            .OnDelete(DeleteBehavior.Cascade);

        // ScheduledMatchAttendance → ScheduledMatch
        modelBuilder.Entity<ScheduledMatchAttendance>()
            .HasOne(attendance => attendance.ScheduledMatch)
            .WithMany(scheduledMatch => scheduledMatch.ScheduledMatchAttendances)
            .HasForeignKey(attendance => attendance.ScheduledMatchId)
            .OnDelete(DeleteBehavior.Cascade);

        // ScheduledMatchAttendance → Player — remove attendance on player delete
        modelBuilder.Entity<ScheduledMatchAttendance>()
            .HasOne(attendance => attendance.Player)
            .WithMany(player => player.ScheduledMatchAttendances)
            .HasForeignKey(attendance => attendance.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        // ImageResource → PlayingField
        modelBuilder.Entity<ImageResource>()
            .HasOne(img => img.PlayingField)
            .WithMany(pf => pf.Images)
            .HasForeignKey(img => img.PlayingFieldId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatMessage>(e =>
        {
            e.HasOne(m => m.Party)
             .WithMany()
             .HasForeignKey(m => m.PartyId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(m => m.Sender)
             .WithMany()
             .HasForeignKey(m => m.SenderUserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
