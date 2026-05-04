using Microsoft.EntityFrameworkCore;
using SpaceAPI.Database;
using SpaceAPI.Services;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "FrontendCors";

// -------------------- MVC Controllers --------------------
builder.Services.AddControllers();

// -------------------- Swagger / OpenAPI --------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -------------------- CORS --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                // Local Vite development
                "http://localhost:5173",
                "http://127.0.0.1:5173",

            // Later, add GitHub Pages here:
                "https://TudorDan.github.io"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// -------------------- NASA typed HttpClient --------------------
builder.Services.AddHttpClient<NasaLibraryClient>(client =>
{
    client.BaseAddress = new Uri("https://images-api.nasa.gov/");
});

// -------------------- Open-Meteo Geocoding --------------------
builder.Services.AddHttpClient<OpenMeteoGeocodingClient>(client =>
{
    client.BaseAddress = new Uri("https://geocoding-api.open-meteo.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

// -------------------- Open-Meteo Forecast --------------------
builder.Services.AddHttpClient<OpenMeteoForecastClient>(client =>
{
    client.BaseAddress = new Uri("https://api.open-meteo.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

// -------------------- SQLite Space DB --------------------
var appDataPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(appDataPath);

var dbPath = Path.Combine(appDataPath, "space.db");

var cs = builder.Configuration.GetConnectionString("SpaceDb")
         ?? $"Data Source={dbPath};Foreign Keys=True";

builder.Services.AddDbContext<SpaceDbContext>(options =>
{
    options.UseSqlite(cs);
});

builder.Services.AddScoped<IPlanetService, PlanetService>();

var app = builder.Build();

// -------------------- Swagger --------------------
// Keep Swagger enabled temporarily for deployment testing.
// Later, you can place this back inside the Development-only condition.
app.UseSwagger();
app.UseSwaggerUI();

// -------------------- HTTPS --------------------
app.UseHttpsRedirection();

// -------------------- Routing / CORS / Controllers --------------------
app.UseRouting();

app.UseCors(FrontendCorsPolicy);

app.MapControllers();

app.Run();