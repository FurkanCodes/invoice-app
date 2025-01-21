// 📂 Infrastructure/InfrastructureExtensions.cs
using InvoiceApp.Infrastructure.Persistence;
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
        // Register PostgreSQL
        services.AddDbContext<AppDbContext>(options =>
         options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}