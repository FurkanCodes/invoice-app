using System.Net;
using System.Net.Mail;
using InvoiceApp.Application.Common.Interfaces;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Infrastructure.Persistence;
using InvoiceApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApp.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                }
            ));
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<AppDbContext>());
        var emailConfig = configuration.GetSection("Email");
        services
        .AddFluentEmail(emailConfig["SenderEmail"])
        .AddSmtpSender(new SmtpClient
        {
            Host = emailConfig["Host"] ?? "localhost",
            Port = int.Parse(emailConfig["Port"] ?? "25"),
            EnableSsl = bool.Parse(emailConfig["EnableSsl"] ?? "false"),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false  // Add this line
        });

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}
