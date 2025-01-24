using System.Reflection;
using InvoiceApp.API.Middleware;
using InvoiceApp.Application;
using InvoiceApp.Infrastructure;
using InvoiceApp.Infrastructure.Persistence;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services from Application and Infrastructure layers
builder.Services
    .AddApplication()    // Registers MediatR and validators
    .AddInfrastructure(builder.Configuration); // 👈 Pass the configuration


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Invoices API",
        Description = "An app that manages invoices",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice API v1");
        c.RoutePrefix = "swagger"; // Default
    });
}

//checks if DB connection is succesfull
app.MapGet("/health", async (AppDbContext dbContext) => 
{
    try 
    {
        await dbContext.Database.CanConnectAsync();
        return Results.Ok("Healthy");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection failed: {ex.Message}");
    }
});

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();