using FintachartsApi.Configuration;
using FintachartsApi.Data;
using FintachartsApi.Services;
using FintachartsApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<FintachartsOptions>(builder.Configuration.GetSection(FintachartsOptions.SectionName));
builder.Services.AddHttpClient<IFintachartsApiService, FintachartsApiService>();

builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddHostedService<FintachartsWebSocketService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    for (int i = 0; i < 5; i++)
    {
        try
        {
            dbContext.Database.Migrate();
            break;
        }
        catch (Exception ex)
        {
            if (i == 4) throw;

            System.Threading.Thread.Sleep(3000);
        }
    }
}

app.Run();
