using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AspNet3.AppLib.StartupExt
{
    public static class ForwardedHeaderOptionsExtensions
    {
        /// <summary>
        /// AddControllersWithViews den sonra
        /// </summary>
        /// <param name="services"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static IServiceCollection _ConfigureForwardedHeaderOptions(this IServiceCollection services)
        {
            //services.Configure<ForwardedHeadersOptions>(options =>
            //{
            //    // options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            //    options.ForwardedHeaders = ForwardedHeaders.All;
            //    options.RequireHeaderSymmetry = false;
            //    options.ForwardLimit = null;
            //});

            return services;
        }

        /// <summary>
        /// En üste exceptionlardan sonra
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder _UseForwardedHeaderOptions(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All,
                RequireHeaderSymmetry = false,
                ForwardLimit = null,
                KnownNetworks = { new IPNetwork(IPAddress.Parse("::ffff:172.17.0.1"), 104) }
            });

            // app.UseForwardedHeaders();

            return app;
        }
    }
}
