
using Microsoft.EntityFrameworkCore;
using SpaceAPI.Models;

namespace SpaceAPI.Database
{
    public class SpaceDbContext : DbContext
    {
        public SpaceDbContext(DbContextOptions<SpaceDbContext> options) : base(options) { }

        public DbSet<Planet> Planets => Set<Planet>();
        public DbSet<Satellite> Satellites => Set<Satellite>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Planet>(entity =>
            {
                entity.ToTable("Planets");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name).IsRequired();
            });

            modelBuilder.Entity<Satellite>(entity =>
            {
                entity.ToTable("Satellites");
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Name).IsRequired();

                entity.HasOne(s => s.Planet)
                    .WithMany(p => p.Satellites)
                    .HasForeignKey(s => s.PlanetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}