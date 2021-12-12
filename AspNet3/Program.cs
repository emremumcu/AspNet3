using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace AspNet3
{
    //Install-Package NLog.Web.AspNetCore 

    public class Program
    {
        public static void Main(string[] args)
        {   
            Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Info("Main");

#if DEBUG
                CreateHostBuilderLocal(args).Build().Run();
#else
                CreateHostBuilder(args).Build().Run();
#endif  
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Main");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        /// <summary>
        /// CreateHostBuilder for localhost (IIS Express) or IIS on Windows
        /// </summary>
        public static IHostBuilder CreateHostBuilderLocal(string[] args) =>
            Host.CreateDefaultBuilder(args)            
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // HACK Change WebRoot and ContentRoot
                    // webBuilder.UseContentRoot(System.IO.Directory.GetCurrentDirectory());
                    // webBuilder.UseWebRoot("wwwroot");
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();

        /// <summary>
        /// CreateHostBuilder for Linux deployment on Kestrel
        /// </summary>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // HACK Change WebRoot and ContentRoot
                    // webBuilder.UseContentRoot(System.IO.Directory.GetCurrentDirectory());
                    // webBuilder.UseWebRoot("wwwroot");
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel();
                    webBuilder.UseUrls("http://localhost:5000");
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog(); 
    }
}
