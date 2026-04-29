using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application;
using Sai.DealAssistant.Application.System.Seeding;
using Sai.DealAssistant.Common.Configuration;
using Sai.DealAssistant.Infrastructure;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.WebApi.Authorizations;
using Sai.DealAssistant.WebApi.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

#region Configure Services
// Add configuration, logging and services
builder.Services.AddCommon();
var myConfig = builder.Services.BuildServiceProvider().GetRequiredService<IAppConfiguration>();

// Log the resolved configuration values
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogInformation("=== Configuration Loaded ===");
logger.LogInformation("Environment: {Environment}", builder.Environment.EnvironmentName);
logger.LogInformation("AppConnection: {AppConnection}", myConfig.AppConnectionString);
logger.LogInformation("MigrationConnection: {MigrationConnection}", myConfig.MigrationConnectionString);
logger.LogInformation("AllowedCorsOrigins: {AllowedCorsOrigins}", myConfig.AllowedCorsOrigins);
logger.LogInformation("============================");

builder.Services.AddInfrastructure(myConfig);
builder.Services.AddApplication();
builder.Services.AddAutoMapper(cfg => { },
    typeof(ApplicationMappingProfile).Assembly,
    typeof(InfrastructureMappingProfile).Assembly
);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // optional, for camelCase
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicies.AllowFrontend, CorsPolicies.AllowFrontendCorsPolicy(myConfig.AllowedCorsOrigins));
});

builder.Services.AddHealthChecks();

#endregion

var app = builder.Build();

app.UseGlobalExceptionHandler();

#region Migrate and Seed database
// Ensure database is migrated on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<AppDbContext>();
        db.Database.SetConnectionString(myConfig.MigrationConnectionString);
        db.Database.Migrate();
        var seeder = services.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
        if (myConfig.SeedTestData)
        {
            await seeder.SeedTestDataAsync();
            if (myConfig.MultiplyDealsTargetRowCount > 0)
            {
                await seeder.MultiplyTestDataAsync(myConfig.MultiplyDealsTargetRowCount);
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
        throw;
    }
}
#endregion

#region ConfigureApp
// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(CorsPolicies.AllowFrontend);

app.MapControllers(); 
app.MapHealthChecks("/health");
#endregion

app.Run();