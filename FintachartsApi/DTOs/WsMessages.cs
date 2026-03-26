using System.Text.Json.Serialization;

namespace FintachartsApi.DTOs
{
    public class WsUpdateMessage
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("instrumentId")]
        public Guid InstrumentId { get; set; }

        [JsonPropertyName("ask")]
        public WsPriceData? Ask { get; set; }
    }

    public class WsPriceData
    {
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }

    public class WsSubscribeRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "l1-subscription";

        [JsonPropertyName("id")]
        public string Id { get; set; } = "1";

        [JsonPropertyName("instrumentId")]
        public Guid InstrumentId { get; set; }

        [JsonPropertyName("provider")]
        public string Provider { get; set; } = "oanda";

        [JsonPropertyName("subscribe")]
        public bool Subscribe { get; set; } = true;

        [JsonPropertyName("kinds")]
        public List<string> Kinds { get; set; } = new() { "ask", "bid", "last" };
    }
}
