namespace FintachartsApi.Models
{
    public class Asset
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Kind { get; set; }
        public decimal? Price { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}
