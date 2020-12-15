using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;

namespace TSC.Web.OpsPortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // base path changes depending on whether we are running as a windows
            // service or locally (via console or vs)
            var basePath = WindowsServiceHelpers.IsWindowsService()
                ? AppContext.BaseDirectory
                : Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Filter.ByExcluding(logEvent =>
                {
                    // exclude hangfire stats endpoints from logging as they just spam the logs
                    var shouldExclude = logEvent.Properties.Any(property =>
                        property.Key == "RequestPath" &&
                        property.Value.ToString().Contains("/stats"));

                    return shouldExclude;
                })
                .CreateLogger();

            try
            {
                Log.Information("Application Starting.");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The Application failed to start.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
                 .UseSerilog()
                 .UseWindowsService()
                 .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder.UseStartup<Startup>();
                 });
    }
}
