namespace FintachartsApi.Configuration;

public class FintachartsOptions
{
    public const string SectionName = "Fintacharts";

    public string ApiBaseUrl { get; set; } = string.Empty;
    public string WsBaseUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}