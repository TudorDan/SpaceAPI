namespace SpaceAPI.DTOs
{
    public class PlanetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public double MeanRadiusKm { get; set; }
        public int Moons { get; set; }
        public string? Description { get; set; }
    }
}