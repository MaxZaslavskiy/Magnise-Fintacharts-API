using System.Text.Json.Serialization;

namespace FintachartsApi.DTOs
{
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }
}
