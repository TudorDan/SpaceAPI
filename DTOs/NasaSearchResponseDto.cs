namespace SpaceAPI.DTOs
{
    public class NasaSearchResponseDto
    {
        public string Query { get; set; } = "";
        public int Returned { get; set; }
        public List<SpaceImageDto> Items { get; set; } = new();
    }
}