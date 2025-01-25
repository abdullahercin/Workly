using Workly.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSwaggerDocumentation();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddApplicationServices();

var app = builder.Build();

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
