using Feijuca.Auth.Api.Tests.Extensions;
using Feijuca.Auth.Api.Tests.Models;
using Feijuca.Auth.Extensions;
using Feijuca.Auth.Http.Client;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.GetSection("Settings").Get<Settings>()!;

builder.Services.AddHttpClient<IFeijucaAuthClient, FeijucaAuthClient>(client =>
{
    client.BaseAddress = new Uri(settings.Feijuca.Url);
});

builder.Services.AddControllers();
builder.Services
    .AddApiAuthentication()
    .AddEndpointsApiExplorer()
    .AddApiSpecification()
    .AddOpenApi("v1");

var app = builder.Build();

app.MapOpenApi();
app.UseSpecification();

app.UseHttpsRedirection();
app.MapControllers();
app.UseTenantMiddleware();

await app.RunAsync();
