using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Serilog;
using Serilog.Formatting.Compact;
using PG_DataAccess.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace PG_Core
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            // Serilog Loging Configuration
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();

            // Serilog and Seq Loging Configuration
            Log.Logger = new LoggerConfiguration()
               .Enrich.FromLogContext()
               .ReadFrom.Configuration(configuration)
               .WriteTo.Console(new RenderedCompactJsonFormatter())
               .WriteTo.Debug(outputTemplate: DateTime.UtcNow.ToString())
               .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
               .WriteTo.Seq("http://seq:5341/")
               .CreateLogger();

            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            // Implemented to print error if SQL Server has an error upon migrating or seeding data. 
            try
            {
                var dbContext = services.GetRequiredService<PgDbContext>();
                if (dbContext.Database.IsSqlServer())
                {
                    dbContext.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                logger.LogError(ex, "An error occurred while migrating or seeding the database.");

                throw;
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}