using Microsoft.AspNetCore.Mvc;
using SpaceAPI.Services;
using SpaceAPI.DTOs;

namespace SpaceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NasaImagesController : ControllerBase
{
    private readonly NasaLibraryClient _nasa;

    public NasaImagesController(NasaLibraryClient nasa) => _nasa = nasa;

    // GET /api/nasaimages/search?q=mars&page=1&take=20&excludeEarth=true
    [HttpGet("search")]
    public async Task<ActionResult<NasaSearchResponseDto>> Search(
        [FromQuery] string q,
        [FromQuery] int page = 1,
        [FromQuery] int take = 20,
        [FromQuery] bool excludeEarth = true,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Query parameter 'q' is required (e.g. mars, jupiter, nebula).");

        var items = await _nasa.SearchImagesAsync(q.Trim(), Math.Max(1, page), ct);

        if (excludeEarth)
        {
            items = items
                .Where(i =>
                    !i.Title.Contains("earth", StringComparison.OrdinalIgnoreCase) &&
                    !(i.Description?.Contains("earth", StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }

        take = Math.Clamp(take, 1, 50);

        var dto = new NasaSearchResponseDto
        {
            Query = q.Trim(),
            Items = items.Take(take).Select(i => new SpaceImageDto
            {
                NasaId = i.NasaId,
                Title = i.Title,
                Description = i.Description,
                PreviewUrl = i.PreviewUrl,
                DateCreated = i.DateCreated
            }).ToList()
        };

        dto.Returned = dto.Items.Count;
        return Ok(dto);
    }
}
