using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Infrastructure.DependencyInjection;
using Sai.DealAssistant.Infrastructure.Persistence;
using Sai.DealAssistant.Application.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add configuration, logging and services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure database is migrated on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var db = services.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
        throw;
    }
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map minimal API endpoints from the application layer
app.MapApplicationEndpoints();

app.Run();