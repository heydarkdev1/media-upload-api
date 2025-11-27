var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Add Authentication + Authorization (Azure AD JWT handling)
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Allow upload streaming (important for large uploads)
builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxRequestBodySize = long.MaxValue;
});

var app = builder.Build();

app.UseAuthentication();   // ?? REQUIRED
app.UseAuthorization();    // ?? REQUIRED

// Map API controllers
app.MapControllers();

app.Run();
