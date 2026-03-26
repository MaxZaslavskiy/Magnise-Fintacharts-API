using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FintachartsApi.Configuration;
using FintachartsApi.DTOs;
using FintachartsApi.Services.Interfaces;
using Microsoft.Extensions.Options;


namespace FintachartsApi.Services;

public class FintachartsApiService : IFintachartsApiService
{
    private readonly HttpClient _httpClient;
    private readonly FintachartsOptions _options;

    public FintachartsApiService(HttpClient httpClient, IOptions<FintachartsOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri(_options.ApiBaseUrl);
    }

    public async Task<string> GetTokenAsync()
    {
        var requestBody = new StringContent(
            $"grant_type=password&client_id=app-cli&username={_options.Username}&password={_options.Password}",
            Encoding.UTF8,
            "application/x-www-form-urlencoded");

        var response = await _httpClient.PostAsync("/identity/realms/fintatech/protocol/openid-connect/token", requestBody);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content);

        return tokenResponse?.AccessToken ?? throw new Exception("Failed to retrieve access token.");
    }

    public async Task<List<FintachartsInstrument>> GetInstrumentsAsync(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync("/api/instruments/v1/instruments?provider=oanda&kind=forex");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var instrumentsResponse = JsonSerializer.Deserialize<InstrumentsResponse>(content);

        return instrumentsResponse?.Data ?? new List<FintachartsInstrument>();
    }
}