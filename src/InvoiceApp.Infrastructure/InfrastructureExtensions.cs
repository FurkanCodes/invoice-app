using System.Net;
using System.Net.Mail;
using InvoiceApp.Application.Common.Interfaces;
using InvoiceApp.Application.Common.Interfaces.Repositories;
using InvoiceApp.Application.Interfaces;
using InvoiceApp.Infrastructure.Persistence;
using InvoiceApp.Infrastructure.Repositories;
using InvoiceApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

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

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
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
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddQuartz();
        services.AddQuartzHostedService(opts => opts.WaitForJobsToComplete = true);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddHttpContextAccessor();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}
