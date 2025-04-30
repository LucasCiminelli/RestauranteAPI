using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Restaurants.Infrastructure.Persistence
{
    public class RestaurantsDbContextFactory : IDesignTimeDbContextFactory<RestaurantsDbContext>
    {
        public RestaurantsDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Restaurants.API");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.Development.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<RestaurantsDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("RestaurantsDb"));

            return new RestaurantsDbContext(optionsBuilder.Options);
        }
    }
}
