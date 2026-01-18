using Microsoft.EntityFrameworkCore;
using Trackly.Models;

namespace Trackly
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<UserModel> Users { get; set; } 
    }
}
