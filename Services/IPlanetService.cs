using SpaceAPI.DTOs;

namespace SpaceAPI.Services
{
    public interface IPlanetService
    {
        Task<List<PlanetDto>> GetAllAsync(CancellationToken ct);
        Task<PlanetDto?> GetByIdAsync(int id, CancellationToken ct);
    }
}