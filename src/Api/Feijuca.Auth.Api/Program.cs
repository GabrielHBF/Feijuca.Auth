using Feijuca.Auth.Extensions;
using Feijuca.Auth.Infra.CrossCutting.Extensions;
using Feijuca.Auth.Infra.CrossCutting.Middlewares;
using Mattioli.Configurations.Extensions.Handlers;
using Mattioli.Configurations.Transformers;
using Scalar.AspNetCore;
using Mattioli.Configurations.Extensions.Telemetry;
using Feijuca.Auth.Common.Models;

var builder = WebApplication.CreateBuilder(args);
var enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.Configuration
    .AddJsonFile("appsettings.json", false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{enviroment}.json", true, reloadOnChange: true)
    .AddEnvironmentVariables();

var applicationSettings = builder.Configuration.ApplyEnvironmentOverridesToSettings(builder.Environment);

builder.Services
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddProblemDetails()
    .AddMediator()
    .AddRepositories()
    .AddValidators()
    .AddServices()
    .AddOpenApi("v1", options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); })
    .AddMongo(applicationSettings.MongoSettings)
    .AddApiAuthentication(out KeycloakSettings KeycloakSettings)
    .AddEndpointsApiExplorer()
    .AddSwagger(KeycloakSettings)
    .AddHttpClients()    
    .ConfigureValidationErrorResponses()
    .AddCors(options =>
    {
        options.AddPolicy("AllowAllOrigins", policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    })
    .AddControllers();

if (!string.IsNullOrEmpty(applicationSettings.MltSettings.OpenTelemetryColectorUrl))
{
    builder.Services.AddOpenTelemetry(applicationSettings.MltSettings);
    builder.ConfigureTelemetryAndLogging(applicationSettings);
}

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options => options.Servers = []);
app.UseCors("AllowAllOrigins")
   .UseExceptionHandler()
   .UseSwagger()
   .UseSwaggerUI(c =>
   {
       c.SwaggerEndpoint("/swagger/v1/swagger.json", "Feijuca.Auth.Api");
   });

if (KeycloakSettings?.Realms?.Any() ?? false)
{
    app.UseAuthorization()
       .UseTenantMiddleware();
}

app.UseHttpsRedirection()
   .UseMiddleware<ConfigValidationMiddleware>();

app.MapControllers();

await app.RunAsync();
