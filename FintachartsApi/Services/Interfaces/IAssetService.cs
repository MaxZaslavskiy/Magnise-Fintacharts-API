using FintachartsApi.Models;

namespace FintachartsApi.Services.Interfaces
{
    public interface IAssetService
    {
        Task SyncAssetsAsync();
        Task<List<Asset>> GetAllAssetsAsync();
        Task<List<Asset>> GetAssetsPricesAsync(string[] symbols);
    }
}
