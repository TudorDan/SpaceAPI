namespace SpaceAPI.DTOs
{
    public sealed class CurrentWeatherDto
    {
        public string Time { get; set; } = "";
        public double TemperatureC { get; set; }
        public int RelativeHumidityPercent { get; set; }
        public double ApparentTemperatureC { get; set; }
        public double PrecipitationMm { get; set; }
        public int WeatherCode { get; set; }
        public double WindSpeedKmh { get; set; }
    }
}