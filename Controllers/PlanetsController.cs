using Microsoft.AspNetCore.Mvc;
using SpaceAPI.DTOs;
using SpaceAPI.Services;

namespace SpaceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanetsController : ControllerBase
    {
        private readonly IPlanetService _service;

        public PlanetsController(IPlanetService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<PlanetDto>>> GetAll(CancellationToken ct) => Ok(await _service.GetAllAsync(ct));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PlanetDto>> GetById(int id, CancellationToken ct)
        {
            var planet = await _service.GetByIdAsync(id, ct);
            return planet is null ? NotFound() : Ok(planet);
        }
    }
}