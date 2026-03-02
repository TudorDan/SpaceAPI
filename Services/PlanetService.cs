using Microsoft.EntityFrameworkCore;
using SpaceAPI.Database;
using SpaceAPI.DTOs;

namespace SpaceAPI.Services
{
    public class PlanetService : IPlanetService
    {
        private readonly SpaceDbContext _db;

        public PlanetService(SpaceDbContext db)
        {
            _db = db;
        }

        public Task<List<PlanetDto>> GetAllAsync(CancellationToken ct) => _db.Planets
          .AsNoTracking() // read-only optimization
          .OrderBy(p => p.Id)
          .Select(p => new PlanetDto
          {
              Id = p.Id,
              Name = p.Name,
              MeanRadiusKm = p.MeanRadiusKm,
              Moons = p.Moons,
              Description = p.Description
          })
          .ToListAsync(ct);

        public Task<PlanetDto?> GetByIdAsync(int id, CancellationToken ct) => _db.Planets
          .AsNoTracking()
          .Where(p => p.Id == id)
          .Select(p => new PlanetDto
          {
              Id = p.Id,
              Name = p.Name,
              MeanRadiusKm = p.MeanRadiusKm,
              Moons = p.Moons,
              Description = p.Description
          })
          .FirstOrDefaultAsync(ct);
    }
}