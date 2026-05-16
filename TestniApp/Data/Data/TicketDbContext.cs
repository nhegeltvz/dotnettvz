using Data.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Data
{
    public class TicketDbContext : IdentityDbContext<User>
    {
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ConfigureTicket(builder);
            ConfigureUser(builder);
            ConfigureAudtiLog(builder);
            ConfigureCateogry(builder);
            ConfigureComment(builder);
        
        }

        private void ConfigureComment(ModelBuilder builder)
        {
            builder.Entity<Comment>()
                .HasOne(comment => comment.Ticket)
                .WithMany(ticket => ticket.Comments)
                .HasForeignKey(comment => comment.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureCateogry(ModelBuilder builder)
        {
            builder.Entity<Category>()
                .HasMany(category => category.Tickets)
                .WithOne(ticket => ticket.Category)
                .HasForeignKey(ticket => ticket.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureAudtiLog(ModelBuilder builder)
        {
            builder.Entity<AuditLog>()
                .HasOne(auditLog => auditLog.Ticket)
                .WithMany(ticket => ticket.AuditLogs)
                .HasForeignKey(auditLog => auditLog.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AuditLog>()
                    .HasOne(auditLog => auditLog.UserChanged)
                    .WithMany(user => user.AuditLogs)
                    .HasForeignKey(user => user.ChangedByUserId)
                    .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureUser(ModelBuilder builder)
        {
            
        }

        private void ConfigureTicket(ModelBuilder builder)
        {
            builder.Entity<Ticket>()
                .HasMany(ticket => ticket.AuditLogs)
                .WithOne(auditLog => auditLog.Ticket)
                .HasForeignKey(ticket => ticket.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasMany(ticket => ticket.Comments)
                .WithOne(comments => comments.Ticket)
                .HasForeignKey(ticket => ticket.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasOne(ticket => ticket.Assignee)
                .WithMany(assignee => assignee.AssignedTickets)
                .HasForeignKey(ticket => ticket.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasOne(ticket => ticket.Reporter)
                .WithMany(assignee => assignee.ReviewingTickets)
                .HasForeignKey(ticket => ticket.ReporterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasOne(ticket => ticket.Category)
                .WithMany(category => category.Tickets)
                .HasForeignKey(ticket => ticket.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
