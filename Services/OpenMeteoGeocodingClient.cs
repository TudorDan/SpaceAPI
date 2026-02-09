using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace SpaceAPI.Services
{
    public class OpenMeteoGeocodingClient
    {
        private readonly HttpClient _http;

        public OpenMeteoGeocodingClient(HttpClient http) => _http = http;

        public async Task<GeocodingResponse?> SearchAsync(
            string name,
            string? countryCode = null,
            int count = 1,
            CancellationToken ct = default)
        {
            var url =
                "v1/search" +
                $"?name={Uri.EscapeDataString(name)}" +
                $"&count={count}" +
                "&language=en" +
                "&format=json" +
                (string.IsNullOrWhiteSpace(countryCode) ? "" : $"&country_code={Uri.EscapeDataString(countryCode)}");

            return await _http.GetFromJsonAsync<GeocodingResponse>(url, cancellationToken: ct);
        }

        public sealed class GeocodingResponse
        {
            [JsonPropertyName("results")] public List<GeocodingResult>? Results { get; set; }
        }

        public sealed class GeocodingResult
        {
            [JsonPropertyName("name")] public string? Name { get; set; }
            [JsonPropertyName("country_code")] public string? CountryCode { get; set; }
            [JsonPropertyName("latitude")] public double Latitude { get; set; }
            [JsonPropertyName("longitude")] public double Longitude { get; set; }
            [JsonPropertyName("timezone")] public string? Timezone { get; set; }
        }
    }
}