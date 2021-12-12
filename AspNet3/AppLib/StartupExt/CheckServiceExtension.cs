using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet3.AppLib.StartupExt
{
    public static class CheckServiceExtension
    {
        public static IServiceCollection _CheckServiceConfiguration(this IServiceCollection services, IWebHostEnvironment env)
        {            
            return services;
        }

        public static IApplicationBuilder _CheckService<T>(this IApplicationBuilder app)
        {
            // to get services configured in ConfigureServices:
            //IServiceProvider serviceProvider = app.ApplicationServices;
            //IAccountService accService = serviceProvider.GetService<IAccountService>();
            //if (accService == null) throw new Exception("Account service is not registered!");

            IServiceProvider serviceProvider = app.ApplicationServices;
            T service = serviceProvider.GetService<T>();
            if (service == null) throw new Exception($"Service {typeof(T)} not registered in ConfigureServices section.");

            return app;
        }
    }
}
