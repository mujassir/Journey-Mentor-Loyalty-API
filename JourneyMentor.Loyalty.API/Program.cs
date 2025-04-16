using FluentValidation;
using FluentValidation.AspNetCore;
using JourneyMentor.Loyalty.API.Middleware;
using JourneyMentor.Loyalty.Application.Features.Points.Commands;
using JourneyMentor.Loyalty.Application.Features.Points.Handlers;
using JourneyMentor.Loyalty.Application.Validators;
using JourneyMentor.Loyalty.Infrastructure.Redis;
using JourneyMentor.Loyalty.Persistence.Context;
using JourneyMentor.Loyalty.Persistence.Repositories.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var _config = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath) // Set base path to app's content root
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
    .AddEnvironmentVariables() // Add environment variables
    .Build();

builder.Services.AddDbContext<LoyaltyDbContext>(options => options.UseSqlite(_config.GetConnectionString("DefaultConnection")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("Redis:ConnectionString").Value;
    options.InstanceName = "JourneyMentorCache_";
});

builder.Services.AddMediatR(typeof(EarnPointsCommandHandler).Assembly);
builder.Services.AddMediatR(typeof(GetEarnedPointsQueryHandler).Assembly);

builder.Services.AddValidatorsFromAssemblyContaining<EarnPointsCommandValidator>();
builder.Services.AddFluentValidationAutoValidation();

// Configure Serilog using appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .WriteTo.File(builder.Configuration["LOG_FILE_PATH"] ?? "Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<IPointsRepository, PointsRepository>();

// JWT Bearer Authentication
builder.Services.AddSingleton<JwtAuthenticationConfiguration>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Retrieve the JwtAuthenticationConfiguration and configure JWT authentication
        var jwtConfig = builder.Services.BuildServiceProvider().GetRequiredService<JwtAuthenticationConfiguration>();
        jwtConfig.ConfigureJwtAuthentication(options);
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Make sure DB and tables are created (for dev use only)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LoyaltyDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting the app...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "App failed to start");
}
finally
{
    Log.CloseAndFlush();
}
