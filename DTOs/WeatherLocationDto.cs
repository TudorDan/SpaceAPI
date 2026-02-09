namespace SpaceAPI.DTOs
{
    public sealed class WeatherLocationDto
    {
        public string Name { get; set; } = "";
        public string? CountryCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Timezone { get; set; } = "";
    }
}