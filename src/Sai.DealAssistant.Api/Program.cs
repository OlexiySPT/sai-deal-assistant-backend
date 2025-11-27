using MediatR;
using Microsoft.EntityFrameworkCore;
using Sai.DealAssistant.Application;
using Sai.DealAssistant.Application.System.Commands;
using Sai.DealAssistant.Common.Configuration;
using Sai.DealAssistant.Infrastructure;
using Sai.DealAssistant.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add configuration, logging and services
builder.Services.AddCommon();
var myConfig = builder.Services.BuildServiceProvider().GetRequiredService<IMyConfiguration>();
builder.Services.AddInfrastructure(myConfig);
builder.Services.AddApplication();
builder.Services.AddAutoMapper(cfg => { },
    typeof(ApplicationMappingProfile).Assembly,
    typeof(InfrastructureMappingProfile).Assembly
);

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
        db.Database.SetConnectionString(myConfig.MigrationConnectionString);
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