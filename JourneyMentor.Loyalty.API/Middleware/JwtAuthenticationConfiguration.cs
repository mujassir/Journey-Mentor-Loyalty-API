using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace JourneyMentor.Loyalty.API.Middleware
{
    public class JwtAuthenticationConfiguration
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtAuthenticationConfiguration> _logger;

        public JwtAuthenticationConfiguration(IConfiguration configuration, ILogger<JwtAuthenticationConfiguration> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void ConfigureJwtAuthentication(JwtBearerOptions options)
        {
            var authConfig = _configuration.GetSection("Authentication");

            options.Authority = authConfig["Authority"];
            options.Audience = authConfig["Audience"];
            options.RequireHttpsMetadata = bool.Parse(authConfig["RequireHttpsMetadata"]);

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    _logger.LogError("Auth failed: {Message}", context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    _logger.LogInformation("Token validated for {Subject}", context.Principal?.Identity?.Name);
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    _logger.LogWarning("Auth challenge triggered: {Error}", context.ErrorDescription);
                    return Task.CompletedTask;
                }
            };

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authConfig["Authority"],
                ValidateAudience = true,
                ValidAudience = authConfig["Audience"],
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        }
    }
}