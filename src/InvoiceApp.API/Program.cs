using InvoiceApp.API.Middleware;
using InvoiceApp.Application;
using InvoiceApp.Infrastructure;

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
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();