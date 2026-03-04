using Feijuca.Auth.Api.Tests.Models;
using Feijuca.Auth.Extensions;
using Feijuca.Auth.Http.Client;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.GetSection("Settings").Get<Settings>()!;

builder.Services.AddHttpClient<IFeijucaAuthClient, FeijucaAuthClient>(client =>
{
    client.BaseAddress = new Uri(settings.Feijuca.ApiUrl);
});

builder.Services.AddControllers();
builder.Services
    .AddApiAuthentication(settings.Feijuca)
    .AddEndpointsApiExplorer()
    .AddOpenApi("v1");

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();
app.MapControllers();
app.UseTenantMiddleware();

await app.RunAsync();
