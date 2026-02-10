using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QrGenerator.Application.Interfaces;
using QrGenerator.Application.Services;
using QrGenerator.BackgroundServices;
using QrGenerator.Infrastructure.Auth;
using QrGenerator.Infrastructure.Data;
using QrGenerator.Infrastructure.QrCodes;
using QrGenerator.Infrastructure.Telegram;

namespace QrGenerator.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IQrCodeService, QrCodeService>();
        services.AddScoped<IQrCodeRepository, QrCodeRepository>();
        services.AddScoped<IQrCodeGenerator, QrCodeGenerator>();
        services.AddScoped<ITelegramSender, TelegramSender>();
        
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.Configure<TelegramOptions>(configuration.GetSection("Telegram"));
        
        var jwtOptions = configuration
            .GetSection("Jwt")
            .Get<JwtOptions>() ?? throw new InvalidOperationException("Jwt configuration section is missing");
        
        services.AddDbContext<QrDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddHttpClient("telegram");
        services.AddHostedService<TelegramPollingService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Secret))
                };
            });

        services.AddAuthorization();
        return services;
    }
}