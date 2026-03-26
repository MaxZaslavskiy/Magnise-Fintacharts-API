using System.Text.Json.Serialization;

namespace FintachartsApi.DTOs
{
    public class InstrumentsResponse
    {
        [JsonPropertyName("data")]
        public List<FintachartsInstrument> Data { get; set; } = new();
    }
    public class FintachartsInstrument
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("kind")]
        public string? Kind { get; set; }
    }
}
