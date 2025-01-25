// Infrastructure/Persistence/DesignTimeDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace InvoiceApp.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
  public AppDbContext CreateDbContext(string[] args)
  {

    var currentDir = Directory.GetCurrentDirectory();
    var solutionDir = Directory.GetParent(currentDir)?.Parent?.FullName;
    var apiProjectPath = Path.Combine(solutionDir!, "src", "InvoiceApp.API");

    IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(apiProjectPath)
        .AddJsonFile("appsettings.json")
        .AddJsonFile("appsettings.Development.json", optional: true)
        .Build();

    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");

    optionsBuilder.UseNpgsql(connectionString);

    return new AppDbContext(optionsBuilder.Options);
  }
}
