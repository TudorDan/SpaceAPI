namespace SpaceAPI.DTOs
{
    public class SatelliteDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public double RadiusKm { get; set; }
        public double? OrbitalPeriodDays { get; set; }
        public int PlanetId { get; set; }
        public string PlanetName { get; set; } = "";
        public string? Description { get; set; }
    }
}