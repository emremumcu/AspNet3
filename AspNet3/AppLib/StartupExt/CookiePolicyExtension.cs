using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet3.AppLib.StartupExt
{
    // https://docs.microsoft.com/en-us/aspnet/core/security/gdpr?view=aspnetcore-3.1
    public static class CookiePolicyExtension
    {
        public static IServiceCollection _AddCookiePolicyOptions(this IServiceCollection services)
        {
            // Global cookie policy
            // Add also: app.UseCookiePolicy()
            services
                .Configure<CookiePolicyOptions>(options =>
                {
                    // The CheckConsentNeeded option of true will prevent any non-essential cookies 
                    // from being sent to the browser (no Set-Cookie header) without the user's explicit permission.
                    // You can either change this behaviour, or mark your cookie as essential by setting the 
                    // IsEssential property to true when creating it: options.Cookie.IsEssential = true;

                    options.CheckConsentNeeded = context => true; // Enable the default cookie consent feature 

                    options.MinimumSameSitePolicy = SameSiteMode.Lax;
                    options.HttpOnly = HttpOnlyPolicy.Always;
                    options.Secure = CookieSecurePolicy.SameAsRequest;
                });

            return services;
        }

        /// <summary>
        /// Before app.UseRouting();
        /// </summary>
        public static IApplicationBuilder _UseCookiePolicy(this IApplicationBuilder app)
        {
            app.UseCookiePolicy();

            return app;
        }
    }
}
