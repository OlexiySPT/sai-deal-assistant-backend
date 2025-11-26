using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sai.DealAssistant.Application.DependencyInjection;
using Sai.DealAssistant.Application.System.Commands;
using Sai.DealAssistant.Infrastructure.DependencyInjection;
using Sai.DealAssistant.Infrastructure.Persistence;

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
        var configuration = services.GetRequiredService<IConfiguration>();
        db.Database.SetConnectionString(configuration.GetConnectionString("MigrationConnection"));
        db.Database.Migrate();
        IMediator mediator = services.GetRequiredService<IMediator>();
        mediator.Send(new SeedDatabaseCommand(app.Environment.IsDevelopment()), CancellationToken.None).Wait();
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