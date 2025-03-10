using InvoiceApp.Shared.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApp.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR and handlers from this assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationExtensions).Assembly));
        services.AddScoped<ICurrencyValidator, CurrencyValidatorService>();
        services.AddHttpContextAccessor();
        return services;
    }
}