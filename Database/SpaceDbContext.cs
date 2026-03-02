
using Microsoft.EntityFrameworkCore;
using SpaceAPI.Models;

namespace SpaceAPI.Database
{
    public class SpaceDbContext : DbContext
    {
        public SpaceDbContext(DbContextOptions<SpaceDbContext> options) : base(options) { }

        public DbSet<Planet> Planets => Set<Planet>();
    }
}