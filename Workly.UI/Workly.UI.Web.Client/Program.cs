using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Workly.UI.Shared.Services;
using Workly.UI.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


// Add device-specific services used by the Workly.UI.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:5001/"),
    DefaultRequestHeaders = { { "Accept", "application/json" } }
});



await builder.Build().RunAsync();
