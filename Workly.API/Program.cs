using Microsoft.EntityFrameworkCore;
using Serilog;
using Workly.API.Extensions;
using Workly.API.Modules;
using Workly.Infrastructure.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

// Serilog Konfigürasyonu
if (!Directory.Exists("Logs"))
{
    Directory.CreateDirectory("Logs");
}

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/info-.log", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
    .WriteTo.File("Logs/error-.log", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
    .CreateLogger();

// Serilog'u Kullan
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSwaggerDocumentation();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<WorklyDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnection")));

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

//Veritabaný oluþtur ve kontrol et
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<WorklyDbContext>();

try
{
    // Veritabanýný güncelle
    dbContext.Database.Migrate();
    Log.Information("Veritabaný baþarýyla güncellendi.");
}
catch (Exception ex)
{
    Log.Error($"Veritabaný güncellenirken hata oluþtu: {ex.Message}");
}


// OpenAPI endpoint'ini ekle
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Swagger UI'ý ekle
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Kullanýcý kimliðini almak için middleware ekle
app.UseMiddleware<UserContextMiddleware>();

// Hata yönetimi middleware'ini ekle
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
