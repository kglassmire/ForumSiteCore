using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Events;

namespace ForumSiteCore.API
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.RollingFile(@".\Logs\log-{Date}.txt")
                .CreateLogger();

            try
            {
                BuildCustomWebHost(args).Run();
                return 0;
            }
            catch(Exception e)
            {
                Log.Fatal(e, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();


        /// <summary>
        /// Manually defined webhost builder that we DO use.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHost BuildCustomWebHost(string[] args)
        {
            return new WebHostBuilder()
             .UseKestrel()
             .UseContentRoot(Directory.GetCurrentDirectory())
             .ConfigureAppConfiguration((hostingContext, config) =>
             {
                 var env = hostingContext.HostingEnvironment;

                 config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                       .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                 config.AddEnvironmentVariables();
             })
             //.ConfigureLogging((hostingContext, logging) =>
             //{
             //    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
             //    logging.AddConsole();
             //    logging.AddDebug();
             //})             
             .UseStartup<Startup>()
             .UseSerilog()
             .Build();
        }
    }
}
