namespace SWP.Dtos.Services
{
    public class ServiceDto
    {
        public int SerId { get; set; }
        public string? SerName { get; set; }
        public decimal? Price { get; set; }

        public string? Description { get; set; }

        public string? FilePath { get; set; }
    }
}
