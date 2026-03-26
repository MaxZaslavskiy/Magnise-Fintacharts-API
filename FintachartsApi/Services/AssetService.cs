using FintachartsApi.Data;
using FintachartsApi.Models;
using FintachartsApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FintachartsApi.Services;

public class AssetService : IAssetService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IFintachartsApiService _fintachartsApi;

    public AssetService(ApplicationDbContext dbContext, IFintachartsApiService fintachartsApi)
    {
        _dbContext = dbContext;
        _fintachartsApi = fintachartsApi;
    }

    public async Task SyncAssetsAsync()
    {
        var token = await _fintachartsApi.GetTokenAsync();

        var instruments = await _fintachartsApi.GetInstrumentsAsync(token);

        foreach (var instrument in instruments)
        {
            var exists = await _dbContext.Assets.AnyAsync(a => a.Symbol == instrument.Symbol);

            if (!exists)
            {
                var newAsset = new Asset
                {
                    Id = instrument.Id,
                    Symbol = instrument.Symbol,
                    Description = instrument.Description,
                    Kind = instrument.Kind
                };

                _dbContext.Assets.Add(newAsset);
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Asset>> GetAllAssetsAsync()
    {
        return await _dbContext.Assets.ToListAsync();
    }

    public async Task<List<Asset>> GetAssetsPricesAsync(string[] symbols)
    {
        return await _dbContext.Assets
            .Where(a => symbols.Contains(a.Symbol))
            .ToListAsync();
    }
}