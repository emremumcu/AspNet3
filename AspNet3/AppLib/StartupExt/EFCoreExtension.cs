using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet3.AppLib.StartupExt
{
    public static class EFCoreExtension
    {
        public static IServiceCollection _ConfigureEF(this IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            // EF-Core:
            // services.AddDbContext<AppDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            // services.AddDbContext<AppDbContext>(options => options.UseSqlite(configurationRoot.GetConnectionString("DefaultConnection")));
            // services.AddDbContext<AppDbContext>(options => options.UseSqlite(configurationRoot.GetSection("ConnectionStrings")["DefaultConnection"]));

            return services;
        }

        public static IApplicationBuilder _UseEF(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
