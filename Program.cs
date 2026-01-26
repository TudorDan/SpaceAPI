using SpaceAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers (în loc de minimal endpoints)
builder.Services.AddControllers(); // controller-based Web API :contentReference[oaicite:2]{index=2}

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Typed HttpClient via IHttpClientFactory (recomandat)
builder.Services.AddHttpClient<NasaLibraryClient>(client =>
{
    client.BaseAddress = new Uri("https://images-api.nasa.gov/"); // NASA Image & Video Library :contentReference[oaicite:3]{index=3}
}); // :contentReference[oaicite:4]{index=4}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers(); // expune rutele controllerelor :contentReference[oaicite:5]{index=5}

app.Run();
