using System.Text.Json;
using SpaceAPI.Models;

namespace SpaceAPI.Services
{
    public class NasaLibraryClient
    {
        private readonly HttpClient _http;

        public NasaLibraryClient(HttpClient http) => _http = http;

        public async Task<List<SpaceImage>> SearchImagesAsync(string q, int page, CancellationToken ct)
        {
            // NASA Image & Video Library: GET /search?q=...&media_type=image&page=... :contentReference[oaicite:6]{index=6}
            var url = $"search?q={Uri.EscapeDataString(q)}&media_type=image&page={page}";

            using var stream = await _http.GetStreamAsync(url, ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            var results = new List<SpaceImage>();

            if (!doc.RootElement.TryGetProperty("collection", out var collection)) return results;
            if (!collection.TryGetProperty("items", out var items) || items.ValueKind != JsonValueKind.Array) return results;

            foreach (var item in items.EnumerateArray())
            {
                // data[0]
                if (!item.TryGetProperty("data", out var dataArr) || dataArr.ValueKind != JsonValueKind.Array) continue;
                var data0 = dataArr[0];

                var nasaId = data0.TryGetProperty("nasa_id", out var idEl) ? idEl.GetString() : null;
                var title = data0.TryGetProperty("title", out var titleEl) ? titleEl.GetString() : null;
                if (string.IsNullOrWhiteSpace(nasaId) || string.IsNullOrWhiteSpace(title)) continue;

                var desc = data0.TryGetProperty("description", out var descEl) ? descEl.GetString() : null;

                DateTimeOffset? created = null;
                if (data0.TryGetProperty("date_created", out var dateEl))
                {
                    var dateStr = dateEl.GetString();
                    if (DateTimeOffset.TryParse(dateStr, out var parsed)) created = parsed;
                }

                // links -> prefer rel="preview"
                string? previewUrl = null;
                if (item.TryGetProperty("links", out var linksArr) && linksArr.ValueKind == JsonValueKind.Array)
                {
                    foreach (var link in linksArr.EnumerateArray())
                    {
                        var rel = link.TryGetProperty("rel", out var relEl) ? relEl.GetString() : null;
                        var href = link.TryGetProperty("href", out var hrefEl) ? hrefEl.GetString() : null;

                        if (string.IsNullOrWhiteSpace(href)) continue;

                        if (string.Equals(rel, "preview", StringComparison.OrdinalIgnoreCase))
                        {
                            previewUrl = href;
                            break;
                        }

                        previewUrl ??= href;
                    }
                }

                results.Add(new SpaceImage
                {
                    NasaId = nasaId!,
                    Title = title!,
                    Description = desc,
                    PreviewUrl = previewUrl,
                    DateCreated = created
                });
            }

            return results;
        }
    }
}