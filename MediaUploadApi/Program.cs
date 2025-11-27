using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------
// 1. Add Azure AD Authentication
// ----------------------------------------------------------
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// ----------------------------------------------------------
// 2. Add Authorization
// ----------------------------------------------------------
builder.Services.AddAuthorization();

// ----------------------------------------------------------
// 3. Add Controllers
// ----------------------------------------------------------
builder.Services.AddControllers();

// ----------------------------------------------------------
// 4. Allow large file upload (Kestrel)
// ----------------------------------------------------------
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue;
});

var app = builder.Build();

// ----------------------------------------------------------
// 5. Enable Authentication + Authorization (ORDER MATTERS)
// ----------------------------------------------------------
app.UseAuthentication();
app.UseAuthorization();

// ----------------------------------------------------------
// 6. Map Controllers
// ----------------------------------------------------------
app.MapControllers();

app.Run();