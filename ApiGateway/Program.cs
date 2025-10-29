using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;
var ocelotConfigFile = environment == "Production" && File.Exists("ocelot.azure.json")
    ? "ocelot.azure.json"
    : "ocelot.json";

builder.Configuration
    .AddJsonFile(ocelotConfigFile, optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddOcelot();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAll");
await app.UseOcelot();
app.Run();