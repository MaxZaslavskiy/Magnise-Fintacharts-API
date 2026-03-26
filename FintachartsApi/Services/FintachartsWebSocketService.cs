using FintachartsApi.Configuration;
using FintachartsApi.Data;
using FintachartsApi.DTOs;
using FintachartsApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql.Replication.PgOutput.Messages;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FintachartsApi.Services;

public class FintachartsWebSocketService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<FintachartsWebSocketService> _logger;
    private readonly FintachartsOptions _options;

    public FintachartsWebSocketService(
        IServiceScopeFactory scopeFactory,
        ILogger<FintachartsWebSocketService> logger,
        IOptions<FintachartsOptions> options)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConnectAndListenAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"WebSocket connection error: {ex.Message}. Reconnecting in 5 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task ConnectAndListenAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var apiService = scope.ServiceProvider.GetRequiredService<IFintachartsApiService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        _logger.LogInformation("Getting token for WebSocket...");
        var token = await apiService.GetTokenAsync();

        using var ws = new ClientWebSocket();
        var wsUri = new Uri($"{_options.WsBaseUrl}/api/streaming/ws/v1/realtime?token={token}");

        _logger.LogInformation($"Connecting to WebSocket: {wsUri}");
        await ws.ConnectAsync(wsUri, stoppingToken);
        _logger.LogInformation("WebSocket connected successfully!");

        var assets = await dbContext.Assets.ToListAsync(stoppingToken);
        foreach (var asset in assets)
        {
            var subscribeRequest = new WsSubscribeRequest { InstrumentId = asset.Id };
            var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(subscribeRequest);

            await ws.SendAsync(new ArraySegment<byte>(jsonBytes), WebSocketMessageType.Text, true, stoppingToken);
            _logger.LogInformation($"Subscribed to updates for: {asset.Symbol}");
        }

        var buffer = new byte[1024 * 4];

        while (ws.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                _logger.LogWarning("Server requested WebSocket closure.");
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", stoppingToken);
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            if (message.Contains("\"type\":\"session\"") || message.Contains("\"provider-status-update\""))
                continue;

            try
            {
                var updateMessage = JsonSerializer.Deserialize<WsUpdateMessage>(message);

                if (updateMessage?.Type == "l1-update" && updateMessage.Ask != null)
                {
                    await UpdateAssetPriceInDbAsync(updateMessage.InstrumentId, updateMessage.Ask.Price, updateMessage.Ask.Timestamp);
                }
            }
            catch (JsonException)
            {
            }
        }
    }

    private async Task UpdateAssetPriceInDbAsync(Guid instrumentId, decimal price, DateTime timestamp)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var asset = await dbContext.Assets.FindAsync(instrumentId);
        if (asset != null)
        {
            asset.Price = price;
            asset.LastUpdate = timestamp.ToUniversalTime();
            await dbContext.SaveChangesAsync();

            _logger.LogInformation($"[PRICE UPDATE] {asset.Symbol}: {price} (Time: {timestamp:HH:mm:ss})");
        }
    }
}