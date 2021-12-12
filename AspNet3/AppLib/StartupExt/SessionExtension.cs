using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet3.AppLib.StartupExt
{
    public static class SessionExtension
    {
        public static IServiceCollection _AddSession(this IServiceCollection services)
        {
            /*  
             *  PM> Install-Package Microsoft.AspNetCore.Session
             *  
             *  To enable the session middleware, Startup must contain:
             *  Any of the IDistributedCache memory caches. (The IDistributedCache implementation is used as a backing store for session.)
             *  A call to AddSession in ConfigureServices.
             *  A call to UseSession in Configure. 
             *  
             *  Add also: app.UseSession()
             *  
             */

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Session uses a cookie to track and identify requests from a single browser. 
                // By default, this cookie is named .AspNetCore.Session, and it uses a path of /. 
                // Because the cookie default doesn't specify a domain, it isn't made available 
                // to the client-side script on the page (because HttpOnly defaults to true).
                // Session Cookie Options:
                // (To override cookie session defaults (global policy))

                // To determine how long a session can be idle before its contents in the server's 
                // cache are abandoned. This property is independent of the cookie expiration.
                options.IdleTimeout = TimeSpan.FromMinutes(10);

                // for security reasons
                options.Cookie.HttpOnly = true;

                // Session state cookies are not essential by default. 
                // Session state isn't functional when tracking is disabled bu user (GDPR). 
                // Make the session cookie Essential:
                options.Cookie.IsEssential = true;

                options.Cookie.Name = ".AspNetCore.Session";
                options.Cookie.Path = "/";
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }

        /// <summary>
        /// Call UseSession after UseRouting and before UseEndpoints
        /// HttpContext.Session is available after session state is configured.
        /// HttpContext.Session can't be accessed before UseSession has been called.
        /// </summary>
        public static IApplicationBuilder _UseSession(this IApplicationBuilder app)
        {
            app.UseSession();

            return app;
        }

        /* 
         * SESSION EXTENSIONS
         * All session data must be serialized to enable a distributed cache scenario, even when using the in-memory cache.
         * Minimal string and number serializers are provided (see the methods and extension methods of ISession). 
         * Complex types must be serialized by the user using another mechanism, such as JSON. 
         * 
         * */



        public static bool HasKey(this ISession session, string key)
        {
            return session.Keys.Any(k => k == key);
        }

        public static void SetKey<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetKey<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
