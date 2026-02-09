namespace SpaceAPI.DTOs
{
    public sealed class WeatherCurrentDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Timezone { get; set; } = "";
        public CurrentWeatherDto Current { get; set; } = new();
    }
}