using AspNet3.AppLib.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet3.AppLib.StartupExt
{
    public static class OptionsExtension
    {
        public static IServiceCollection _AddOptions(this IServiceCollection services, IConfigurationRoot ConfigurationRoot)
        {



            services.AddOptions();
            services.Configure<Config>(ConfigurationRoot.GetSection("Configuration"));            

            return services;



            /*
 the name you specify in GetSection("MySettings") maps to a top level property in the JSON file. Anything below is pulled into your configuration object when the configuration is read.

Using String Configuration Values
Configuration.GetValue<string>("MySettings:ApplicationName");
 */

            /*
             AddOptions() adds the basic support for injecting IOptions<T> based objects into your code filled with the configuration data from the store. You then register your actual configuration class and map it to the configuration section it should use to read the data from.
            */
            // Add functionality to inject IOptions<T>
            //services.AddOptions();

            // Add our Config object so it can be injected
            //services.Configure<MySettings>(Configuration.GetSection("MySettings"));

            /*
             This is unrelated to using the strongly typed class and IOptions, but when you need inject a more generic IConfiguration for using string based configuration, make sure to explicitly register IConfiguration with the DI system. If you don't you get this error:

InvalidOperationException: Unable to resolve service for type 'Microsoft.Extensions.Configuration.IConfiguration' while attempting to activate 'WebApplication8.Controllers.ConfigurationController'.
             */
            // *If* you need access to generic IConfiguration this is **required**
            //services.AddSingleton<IConfiguration>(Configuration);



            /*
             Refreshing Configuration Values
Unlike standard ASP.NET applications, ASP.NET Core applications don't automatically refresh the configuration data when the data is changed in the configuration store.

This means if you make a change while the application is running, you either need to restart the application, or explicitly refresh the configuration values.
             

            services.AddSingleton<IConfigurationRoot>(Configuration);   // IConfigurationRoot
services.AddSingleton<IConfiguration>(Configuration);   // IConfiguration explicitly



            private IConfiguration Configuration { get; set; }
private MySettings MySettings { get; set; }
private IConfigurationRoot ConfigRoot { get; set; }

public ConfigurationController(IOptions<MySettings> settings, IConfiguration configuration,IConfigurationRoot configRoot)
{            
    MySettings = settings.Value;
    Configuration = configuration;
    
    ConfigRoot = configRoot;
}


            public ActionResult ReloadConfig()
{
    ConfigRoot.Reload();

    // this should give the latest value from config
    var lastVal = Configuration.GetValue<string>("MySettings:ApplicationName");
    
    return Ok(lastVal); 
}

             */

        }

        public static IApplicationBuilder _UseOptions(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
