var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Allow upload streaming (important for large uploads)
builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxRequestBodySize = long.MaxValue;
});

var app = builder.Build();

// Map API controllers
app.MapControllers();

app.Run();
