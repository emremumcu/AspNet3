using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AspNet3.AppLib.Evaluators;
using AspNet3.AppLib.Filters;

using AspNet3.AppLib.StartupExt;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace AspNet3
{
    public class Startup
    {
        private IWebHostEnvironment Env { get; }

        // This package contains the Interfaces and functionality for retrieving config values/sections.
        public IConfiguration Configuration { get; set; }

        // The only reason you really ever need IConfigurationRoot is if you want to reload config, or want access to the individual providers that make up the config
        public IConfigurationRoot ConfigurationRoot { get; set; }
        
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            // Building Configuration manually:
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            //    .AddEnvironmentVariables();

            //Configuration = builder.Build();

            Env = env;
            Configuration = configuration;

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("config.json", optional: true, reloadOnChange: true)
                .AddJsonFile("data.json", optional: true, reloadOnChange: true)
            ;

            ConfigurationRoot = builder.Build();

            /*
             * Getting values from configuration
             * ---------------------------------
             * { "Version": "1.0.0" }
             * var version = configuration["Version"];
             * 
             * { "Auth": { "Users": ["user1", "user2"] } }
             * var users = configuration.GetSection("Auth:Users")[.GetChildren()]
             * 
             */
        }

        /// <summary>
        /// When the application is requested for the first time, it calls ConfigureServices method. 
        /// ASP.net core has built-in support for Dependency Injection. We can add services to DI container using this method.
        /// Use ConfigureServices method to configure Dependency Injection (services).
        /// At run time, the ConfigureServices method is called before the Configure method. 
        /// This is so that you can register your custom service with the IoC container which you may use in the Configure method.
        /// Singleton: IoC container will create and share a single instance of a service throughout the application's lifetime.
        /// Transient: The IoC container will create a new instance of the specified service type every time you ask for it.
        /// Scoped: IoC container will create an instance of the specified service type once per request and will be shared in a single request.
        /// services.Add(new ServiceDescriptor(typeof(ILog), new MyConsoleLogger()));    // singleton
        /// services.Add(new ServiceDescriptor(typeof(ILog), typeof(MyConsoleLogger), ServiceLifetime.Transient)); // Transient
        /// services.Add(new ServiceDescriptor(typeof(ILog), typeof(MyConsoleLogger), ServiceLifetime.Scoped));    // Scoped
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public void ConfigureServices(IServiceCollection services)
        {
            services._InitMVC(Env);

            services._ConfigureForwardedHeaderOptions();

            services._AddCookiePolicyOptions();

            services._AddSession();            

            services._ConfigureViewLocationExpander();

            services._AddAuthentication();

            services._AddAuthorization();

            services._AddOptions(ConfigurationRoot);            

            services._Mock(Env, Configuration, ConfigurationRoot);

            services._ConfigureEF(ConfigurationRoot);

            services.AddMemoryCache();

            services._AddCORS();

            services.AddScoped<WebExceptionFilter>();




        }

        /// <summary>
        /// This method is used to define how the application will respond on each HTTP request.
        /// This method is also used to configure middleware in HTTP pipeline.
        /// Use Configure method to set up middlewares, routing rules etc.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            // if (env.IsDevelopment() || env.IsStaging()) app._UseDevExceptions(); else app._UseProdExceptions();

            app._InitApp();

            app._UseCookiePolicy();

            app._UseSCP();

            app._UseForwardedHeaderOptions();

            app._CheckService<IWebHostEnvironment>();


            // Short circuit
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});

            app._Mock();

            app.UseStaticFiles();



            app.UseRouting();

            app._UseCORS();

            app._UseSession();

            app._UseAuthentication();
            
            app._UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            /*
             * The big problem with the AuthorizeFilter approach is that it's an MVC-only feature. 
             * ASP.NET Core 3.0+ provides a different mechanism for setting authorization on endpoints the 
             * RequireAuthorization() extension method on IEndpointConventionBuilder.
             * 
             * Instead of configuring a global AuthorizeFilter, call RequireAuthorization() when configuring 
             * the endpoints of your application, in Configure():
             */
            ////app.UseEndpoints(endpoints =>
            ////{
            ////    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}").RequireAuthorization();
            ////    endpoints.MapHealthChecks("/health").RequireAuthorization();
            ////    endpoints.MapRazorPages().RequireAuthorization("MyCustomPolicy");
            ////    endpoints.MapHealthChecks("/healthz").RequireAuthorization("OtherPolicy", "MainPolicy");
            ////});
        }
    }
}


/*
    There are three ways to get an instance of IServiceProvider:
    ------------------------------------------------------------
    Using IApplicationBuilder

    public void Configure(IServiceProvider pro, IApplicationBuilder app, IHostingEnvironment env)
    {
        var services = app.ApplicationServices;
        var logger = services.GetService<ILog>();
    }

    Using HttpContext

    var services = HttpContext.RequestServices;
    var log = (ILog)services.GetService(typeof(ILog));

    Using IServiceCollection

    public void ConfigureServices(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
    } 
 */

// ------------------------------------------------------------------------------------------------------------
// The following Startup.Configure method adds security-related middleware components in the recommended order:
// ------------------------------------------------------------------------------------------------------------

//public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//{
//    if (env.IsDevelopment()) {
//        app.UseDeveloperExceptionPage();
//        app.UseDatabaseErrorPage();
//    }
//    else {
//        app.UseExceptionHandler("/Error");
//        app.UseHsts();
//    }
//    app.UseHttpsRedirection();
//    app.UseStaticFiles();
//    app.UseCookiePolicy();
//    app.UseRouting();
//    app.UseRequestLocalization();
//    app.UseCors();
//    app.UseAuthentication();
//    app.UseAuthorization();
//    app.UseSession();
//    app.UseResponseCaching();
//    app.UseEndpoints(endpoints =>
//    {
//        endpoints.MapRazorPages();
//        endpoints.MapControllerRoute(
//            name: "default",
//            pattern: "{controller=Home}/{action=Index}/{id?}");
//    });
//}