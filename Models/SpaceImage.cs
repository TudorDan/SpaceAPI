namespace SpaceAPI.Models
{
    public class SpaceImage
    {
        public string NasaId { get; set; } = "";
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string? PreviewUrl { get; set; }
        public DateTimeOffset? DateCreated { get; set; }
    }
}