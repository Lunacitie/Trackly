using Microsoft.EntityFrameworkCore;
using Trackly.Models;

namespace Trackly
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<HabitModel> Habits { get; set; }
        public DbSet<HabitEntryModel> HabitEntries { get; set; }
        public DbSet<NoteModel> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HabitEntryModel>()
                .HasIndex(x => new { x.HabitId, x.Date })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
