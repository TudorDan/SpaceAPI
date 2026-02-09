using Microsoft.AspNetCore.Mvc;
using SpaceAPI.DTOs;
using SpaceAPI.Services;

namespace SpaceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly OpenMeteoForecastClient _forecast;
    private readonly OpenMeteoGeocodingClient _geo;

    public WeatherController(OpenMeteoForecastClient forecast, OpenMeteoGeocodingClient geo)
    {
        _forecast = forecast;
        _geo = geo;
    }

    // GET /api/weather/current?lat=...&lon=...&timezone=Europe/Bucharest
    [HttpGet("current")]
    [ProducesResponseType(typeof(WeatherCurrentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<WeatherCurrentDto>> GetCurrent([FromQuery] double lat, [FromQuery] double lon,
        [FromQuery] string timezone = "auto", CancellationToken ct = default)
    {
        if (lat is < -90 or > 90) return BadRequest("lat must be between -90 and 90.");
        if (lon is < -180 or > 180) return BadRequest("lon must be between -180 and 180.");

        var data = await _forecast.GetCurrentAsync(lat, lon, timezone, ct);
        var dto = MapToWeatherCurrentDto(data);

        return dto is null
            ? StatusCode(StatusCodes.Status502BadGateway, "Weather provider returned no current weather.")
            : Ok(dto);
    }

    // GET /api/weather/current-by-city?city=Bucharest&countryCode=RO
    [HttpGet("current-by-city")]
    [ProducesResponseType(typeof(WeatherCurrentByCityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<WeatherCurrentByCityDto>> GetCurrentByCity([FromQuery] string city,
        [FromQuery] string? countryCode = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(city))
            return BadRequest("city is required.");

        var geo = await _geo.SearchAsync(city, countryCode, count: 1, ct);
        var hit = geo?.Results?.FirstOrDefault();

        if (hit is null)
            return NotFound("City not found.");

        var tz = string.IsNullOrWhiteSpace(hit.Timezone) ? "auto" : hit.Timezone;

        var forecast = await _forecast.GetCurrentAsync(hit.Latitude, hit.Longitude, tz, ct);
        var weatherDto = MapToWeatherCurrentDto(forecast);

        if (weatherDto is null)
            return StatusCode(StatusCodes.Status502BadGateway, "Weather provider returned no current weather.");

        var result = new WeatherCurrentByCityDto
        {
            Location = new WeatherLocationDto
            {
                Name = hit.Name ?? city,
                CountryCode = hit.CountryCode,
                Latitude = hit.Latitude,
                Longitude = hit.Longitude,
                Timezone = tz
            },
            Weather = weatherDto
        };

        return Ok(result);
    }

    private static WeatherCurrentDto? MapToWeatherCurrentDto(OpenMeteoForecastClient.ForecastResponse? src)
    {
        var c = src?.Current;
        if (src is null || c is null) return null;

        // enforce our DTO contract: if provider omits fields, treat as 502
        if (string.IsNullOrWhiteSpace(src.Timezone) ||
            string.IsNullOrWhiteSpace(c.Time) ||
            c.Temperature2m is null ||
            c.RelativeHumidity2m is null ||
            c.ApparentTemperature is null ||
            c.Precipitation is null ||
            c.WeatherCode is null ||
            c.WindSpeed10m is null)
        {
            return null;
        }

        return new WeatherCurrentDto
        {
            Latitude = src.Latitude,
            Longitude = src.Longitude,
            Timezone = src.Timezone!,
            Current = new CurrentWeatherDto
            {
                Time = c.Time!,
                TemperatureC = c.Temperature2m.Value,
                RelativeHumidityPercent = c.RelativeHumidity2m.Value,
                ApparentTemperatureC = c.ApparentTemperature.Value,
                PrecipitationMm = c.Precipitation.Value,
                WeatherCode = c.WeatherCode.Value,
                WindSpeedKmh = c.WindSpeed10m.Value
            }
        };
    }
}
