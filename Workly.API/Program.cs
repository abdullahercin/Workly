using Microsoft.EntityFrameworkCore;
using Serilog;
using Workly.API.Extensions;
using Workly.API.Modules;
using Workly.Infrastructure.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

// Serilog Konfig�rasyonu
if (!Directory.Exists("Logs"))
{
    Directory.CreateDirectory("Logs");
}

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/info-.log", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
    .WriteTo.File("Logs/error-.log", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
    .CreateLogger();

// Burada servisleri ekleyebilirsiniz
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
            builder =>
            {
                builder.WithOrigins("https://localhost:7193")  // Blazor UI URL
                      .AllowCredentials() // Cookie ve authorization header gönderimine izin ver
                       .AllowAnyMethod() // Tüm HTTP metodlarına izin ver
                       .AllowAnyHeader(); // Tüm header'lara izin ver
            });
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS için zorunlu
    options.Cookie.SameSite = SameSiteMode.None; // Cross-Origin istekler için gerekli
});


// Serilog'u Kullan
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSwaggerDocumentation();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<WorklyDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnection")));

builder.Services.AddIdentityServices();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();


builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

//Veritaban� olu�tur ve kontrol et
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<WorklyDbContext>();
app.UseCors("AllowAllOrigins"); // CORS'u etkinleştir


try
{
    // Veritaban�n� g�ncelle
    dbContext.Database.Migrate();
    Log.Information("Veritaban� ba�ar�yla g�ncellendi.");
}
catch (Exception ex)
{
    Log.Error($"Veritaban� g�ncellenirken hata olu�tu: {ex.Message}");
}


// OpenAPI endpoint'ini ekle
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Swagger UI'� ekle
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Kullan�c� kimli�ini almak i�in middleware ekle
app.UseMiddleware<UserContextMiddleware>();

// Hata y�netimi middleware'ini ekle
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
