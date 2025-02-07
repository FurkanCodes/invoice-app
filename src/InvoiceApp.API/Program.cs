// using System.Reflection;
// using System.Text;
// using InvoiceApp.API.Middleware;
// using InvoiceApp.Application;
// using InvoiceApp.Infrastructure;
// using InvoiceApp.Infrastructure.Persistence;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.IdentityModel.Tokens;
// using Microsoft.OpenApi.Models;

// var builder = WebApplication.CreateBuilder(args);

// // Validate configuration
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// if (string.IsNullOrEmpty(connectionString))
// {
//     throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// }

// var jwtSecret = builder.Configuration["JwtSettings:Secret"];
// if (string.IsNullOrEmpty(jwtSecret))
// {
//     throw new InvalidOperationException("JWT Secret is not configured.");
// }

// // Add services to container
// builder.Services
//     .AddApplication()
//     .AddInfrastructure(builder.Configuration);

// builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(options =>
// {
//     options.SwaggerDoc("v1", new OpenApiInfo
//     {
//         Version = "v1",
//         Title = "Invoices API",
//         Description = "An app that manages invoices",
//         TermsOfService = new Uri("https://example.com/terms"),
//         Contact = new OpenApiContact
//         {
//             Name = "Example Contact",
//             Url = new Uri("https://example.com/contact")
//         },
//         License = new OpenApiLicense
//         {
//             Name = "Example License",
//             Url = new Uri("https://example.com/license")
//         }
//     });

//     // Add JWT Authentication to Swagger
//     options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//     {
//         Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n" +
//                     "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
//                     "Example: \"Bearer 12345abcdef\"",
//         Name = "Authorization",
//         In = ParameterLocation.Header,
//         Type = SecuritySchemeType.Http, // Changed from ApiKey to Http
//         Scheme = "Bearer",  // Specify Bearer scheme
//         BearerFormat = "JWT" // Specify JWT format
//     });

//     options.AddSecurityRequirement(new OpenApiSecurityRequirement
//     {
//         {
//             new OpenApiSecurityScheme
//             {
//                 Reference = new OpenApiReference
//                 {
//                     Type = ReferenceType.SecurityScheme,
//                     Id = "Bearer"
//                 }
//             },
//             Array.Empty<string>()
//         }
//     });

//     var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//     options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
// });

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(
//                 Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!)
//             ),
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
//             ValidAudience = builder.Configuration["JwtSettings:Audience"],
//             ValidateLifetime = true,
//             ClockSkew = TimeSpan.Zero,
//             NameClaimType = "name",
//             RoleClaimType = "role"
//         };
//     });

// builder.Services.AddAuthorization();

// var app = builder.Build();




//     app.UseSwagger();
//     app.UseSwaggerUI(c =>
//   {
//       c.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice API v1");
//       c.RoutePrefix = "swagger";
//       c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
//       c.DefaultModelsExpandDepth(-1);
//       c.EnableDeepLinking();
//       c.DisplayRequestDuration();
//   });


// // Health check endpoint
// app.MapGet("/health", async (AppDbContext dbContext) =>
// {
//     try
//     {
//         await dbContext.Database.CanConnectAsync();
//         return Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
//     }
//     catch (Exception ex)
//     {
//         return Results.Problem(
//             title: "Unhealthy",
//             detail: $"Database connection failed: {ex.Message}",
//             statusCode: 503);
//     }
// }).WithDescription("Health check endpoint")
//   .WithOpenApi();

// app.UseMiddleware<ExceptionHandlingMiddleware>();
// app.UseForwardedHeaders();
// app.UseHttpsRedirection();
// app.UseAuthentication();
// app.UseAuthorization();
// app.MapControllers();

// await app.RunAsync();
using System.Reflection;
using System.Text;
using InvoiceApp.API.Middleware;
using InvoiceApp.Application;
using InvoiceApp.Infrastructure;
using InvoiceApp.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Validate configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var jwtSecret = builder.Configuration["JwtSettings:Secret"] 
    ?? throw new InvalidOperationException("JWT Secret is not configured.");

// Layer Registration
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

// Controllers & API Exploration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger Configuration (Modified)
builder.Services.AddSwaggerGen(options =>
{
    // Your existing Swagger documentation setup
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Invoices API",
        Description = "An app that manages invoices",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact { /* ... */ },
        License = new OpenApiLicense { /* ... */ }
    });

    // JWT Security Definition (Keep this!)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    // Security Requirement (Keep this!)
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // XML Documentation (Keep this!)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// JWT Authentication (Keep this!)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)
            ),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Critical Fix: Remove environment check for Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice API v1");
    c.RoutePrefix = "swagger";
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    c.DefaultModelsExpandDepth(-1);
    c.EnableDeepLinking();
    c.DisplayRequestDuration();
});

// Keep your health check endpoint
app.MapGet("/health", async (AppDbContext dbContext) =>
{
    try
    {
        await dbContext.Database.CanConnectAsync();
        return Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Unhealthy",
            detail: $"Database connection failed: {ex.Message}",
            statusCode: 503);
    }
}).WithOpenApi();

// Middleware chain (Keep all these!)
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();