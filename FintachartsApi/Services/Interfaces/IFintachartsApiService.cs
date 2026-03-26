using FintachartsApi.DTOs;

namespace FintachartsApi.Services.Interfaces
{
    public interface IFintachartsApiService
    {
        Task<string> GetTokenAsync();
        Task<List<FintachartsInstrument>> GetInstrumentsAsync(string token);
    }
}
