using SpaceAPI.Services;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "FrontendCors";

// -------------------- MVC Controllers --------------------
builder.Services.AddControllers();

// -------------------- Swagger / OpenAPI --------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -------------------- CORS (allow Vite dev server) --------------------
// Vite default dev server port is 5173.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173"
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

// Open-Meteo (Geocoding)
builder.Services.AddHttpClient<OpenMeteoGeocodingClient>(client =>
{
    client.BaseAddress = new Uri("https://geocoding-api.open-meteo.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

// Open-Meteo (Forecast)
builder.Services.AddHttpClient<OpenMeteoForecastClient>(client =>
{
    client.BaseAddress = new Uri("https://api.open-meteo.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

// -------------------- Dev tools --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// NOTE:
// If your frontend calls the API using http:// (and your API redirects to https://),
// the browser preflight can fail due to redirects.
// Prefer calling the https:// API URL from the frontend/proxy if you keep this enabled.
app.UseHttpsRedirection();

// CORS middleware should run after routing and before endpoints (controllers)
app.UseRouting();
app.UseCors(FrontendCorsPolicy);

app.MapControllers();

app.Run();
