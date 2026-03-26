using FintachartsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FintachartsApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Models.Asset> Assets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Asset>().HasKey(a => a.Id);

            modelBuilder.Entity<Asset>().HasIndex(a => a.Symbol).IsUnique();
        }

    }
}
