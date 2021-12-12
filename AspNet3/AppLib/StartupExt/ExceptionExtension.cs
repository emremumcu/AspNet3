using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet3.AppLib.StartupExt
{
    public static class ExceptionExtension
    {
        public static IServiceCollection _ConfigureExceptions(this IServiceCollection services)
        {
            return services;
        }

        public static IApplicationBuilder _UseDevExceptions(this IApplicationBuilder app)
        {
            // either use services options.Filters.Add<ExceptionActionFilter>(); or this:
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                var result = Newtonsoft.Json.JsonConvert.SerializeObject(new { error = exception.Message });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }));

            return app;
        }

        public static IApplicationBuilder _UseProdExceptions(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                var result = Newtonsoft.Json.JsonConvert.SerializeObject(new { error = exception.Message });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }));


            //app.UseExceptionHandler("/Account/Error");
            app.UseHsts();
            //app.UseExceptionHandler(errorApp =>
            //{
            //    errorApp.Run(async context =>
            //    {
            //        context.Response.StatusCode = 500;
            //        context.Response.ContentType = "text/html";

            //        await context.Response.WriteAsync("<html lang=\"en\"><body>\r\n");
            //        await context.Response.WriteAsync("ERROR!<br><br>\r\n");

            //        var exceptionHandlerPathFeature =
            //            context.Features.Get<IExceptionHandlerPathFeature>();

            //        // Use exceptionHandlerPathFeature to process the exception (for example, 
            //        // logging), but do NOT expose sensitive error information directly to 
            //        // the client.

            //        if (exceptionHandlerPathFeature?.Error is System.IO.FileNotFoundException)
            //        {
            //            await context.Response.WriteAsync("File error thrown!<br><br>\r\n");
            //        }

            //        await context.Response.WriteAsync("<a href=\"/\">Home</a><br>\r\n");
            //        await context.Response.WriteAsync("</body></html>\r\n");
            //        await context.Response.WriteAsync(new string(' ', 512)); // IE padding
            //    });
            //});

            return app;
        }
    }
}
