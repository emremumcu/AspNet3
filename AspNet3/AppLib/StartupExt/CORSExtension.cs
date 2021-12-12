using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet3.AppLib.StartupExt
{
    public static class CORSExtension
    {
        public static IServiceCollection _AddCORS(this IServiceCollection services)
        {
            // cors: Two URLs have the same origin if they have identical schemes, hosts, and ports like: https://example.com/foo.html and https://example.com/bar.html
            // These URLs have different origins than the previous two URLs:
            // https://example.net – Different domain
            // https://www.example.com/foo.html – Different subdomain
            // http://example.com/foo.html – Different scheme
            // https://example.com:9000/foo.html – Different port
            // https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.0
            services.AddCors(options =>
            {
                options.AddDefaultPolicy
                (
                    p => { p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); }
                );

                options.AddPolicy("allowed_origins",
                builder =>
                {
                    // allows specified orgins with any header & method
                    builder.WithOrigins("http://example.com", "http://www.contoso.com").AllowAnyHeader().AllowAnyMethod();

                    // allows anything
                    //builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            return services;
        }

        public static IApplicationBuilder _UseCORS(this IApplicationBuilder app)
        {
            app.UseCors(); // default policy

            return app;
        }
    }
}
