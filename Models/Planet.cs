namespace SpaceAPI.Models
{
    public class Planet
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public double MeanRadiusKm { get; set; }
        public int Moons { get; set; }
        public string? Description { get; set; }

        public ICollection<Satellite> Satellites { get; set; } = new List<Satellite>();
    }
}