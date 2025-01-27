using Microsoft.EntityFrameworkCore;
using Workly.API.Extensions;
using Workly.Infrastructure.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSwaggerDocumentation();

builder.Services.AddDbContext<WorklyDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnection")));

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddApplicationServices();

var app = builder.Build();

//Veritabaný oluþtur ve kontrol et
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<WorklyDbContext>();

try
{
    // Veritabanýný güncelle
    dbContext.Database.Migrate();
    Console.WriteLine("Veritabaný baþarýyla güncellendi.");
}
catch (Exception ex)
{
    Console.WriteLine($"Veritabaný güncellenirken hata oluþtu: {ex.Message}");
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

app.MapControllers();

app.Run();
