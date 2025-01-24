using InvoiceApp.API.Middleware;
using InvoiceApp.Application;
using InvoiceApp.Infrastructure;
using InvoiceApp.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);


// Add services from Application and Infrastructure layers
builder.Services
    .AddApplication()    // Registers MediatR and validators
    .AddInfrastructure(builder.Configuration); // 👈 Pass the configuration


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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