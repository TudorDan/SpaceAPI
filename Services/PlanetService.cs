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

        public Task<List<SatelliteDto>> GetAllSatellitesAsync(CancellationToken ct) =>
            _db.Satellites
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .Select(s => new SatelliteDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    RadiusKm = s.RadiusKm,
                    OrbitalPeriodDays = s.OrbitalPeriodDays,
                    PlanetId = s.PlanetId,
                    PlanetName = s.Planet.Name,
                    Description = s.Description
                })
                .ToListAsync(ct);

        public Task<SatelliteDto?> GetSatelliteByIdAsync(int id, CancellationToken ct) =>
            _db.Satellites
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new SatelliteDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    RadiusKm = s.RadiusKm,
                    OrbitalPeriodDays = s.OrbitalPeriodDays,
                    PlanetId = s.PlanetId,
                    PlanetName = s.Planet.Name,
                    Description = s.Description
                })
                .FirstOrDefaultAsync(ct);
    }
}