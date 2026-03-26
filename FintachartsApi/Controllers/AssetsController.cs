using FintachartsApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FintachartsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _assetService;

    public AssetsController(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncAssets()
    {
        await _assetService.SyncAssetsAsync();
        return Ok(new { Message = "Assets synchronized successfully." });
    }

    [HttpGet]
    public async Task<IActionResult> GetSupportedAssets()
    {
        var assets = await _assetService.GetAllAssetsAsync();
        return Ok(assets);
    }

    [HttpGet("prices")]
    public async Task<IActionResult> GetPrices([FromQuery] string[] symbols)
    {
        if (symbols == null || symbols.Length == 0)
        {
            return BadRequest(new { Message = "Please provide at least one symbol. Example: ?symbols=EURUSD&symbols=AUDCAD" });
        }

        var assets = await _assetService.GetAssetsPricesAsync(symbols);

        var result = assets.Select(a => new {
            a.Symbol,
            Price = a.Price,
            LastUpdate = a.LastUpdate
        });

        return Ok(result);
    }
}