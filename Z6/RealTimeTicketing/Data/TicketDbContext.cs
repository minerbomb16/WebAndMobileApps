using Microsoft.EntityFrameworkCore;
using RealTimeTicketing.Models;

namespace RealTimeTicketing.Data
{
    public class TicketDbContext : DbContext
    {
        public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options) { }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships and constraints
            modelBuilder.Entity<Ticket>().HasKey(t => t.Id);
            modelBuilder.Entity<Message>().HasKey(m => m.Id);

            modelBuilder.Entity<Message>()
                .HasOne<Ticket>()
                .WithMany()
                .HasForeignKey(m => m.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
