using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace SpaceAPI.Services
{
    public class OpenMeteoForecastClient
    {
        private readonly HttpClient _http;

        public OpenMeteoForecastClient(HttpClient http) => _http = http;

        public async Task<ForecastResponse?> GetCurrentAsync(
            double latitude,
            double longitude,
            string timezone = "auto",
            CancellationToken ct = default)
        {
            // Keep it small: only "current" fields you likely need
            var url =
                "v1/forecast" +
                $"?latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                $"&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                "&current=temperature_2m,relative_humidity_2m,apparent_temperature,precipitation,weather_code,wind_speed_10m" +
                $"&timezone={Uri.EscapeDataString(timezone)}";

            return await _http.GetFromJsonAsync<ForecastResponse>(url, cancellationToken: ct);
        }

        public sealed class ForecastResponse
        {
            [JsonPropertyName("latitude")] public double Latitude { get; set; }
            [JsonPropertyName("longitude")] public double Longitude { get; set; }
            [JsonPropertyName("timezone")] public string? Timezone { get; set; }

            [JsonPropertyName("current")] public CurrentWeather? Current { get; set; }
        }

        public sealed class CurrentWeather
        {
            [JsonPropertyName("time")] public string? Time { get; set; }

            [JsonPropertyName("temperature_2m")] public double? Temperature2m { get; set; }
            [JsonPropertyName("relative_humidity_2m")] public int? RelativeHumidity2m { get; set; }
            [JsonPropertyName("apparent_temperature")] public double? ApparentTemperature { get; set; }
            [JsonPropertyName("precipitation")] public double? Precipitation { get; set; }

            // Open-Meteo uses numeric weather codes (WMO). You can map later if you want labels.
            [JsonPropertyName("weather_code")] public int? WeatherCode { get; set; }

            [JsonPropertyName("wind_speed_10m")] public double? WindSpeed10m { get; set; }
        }
    }
}