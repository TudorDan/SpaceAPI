namespace SpaceAPI.DTOs
{
    public class WeatherCurrentByCityDto
    {
        public WeatherLocationDto Location { get; set; } = new();
        public WeatherCurrentDto Weather { get; set; } = new();
    }
}