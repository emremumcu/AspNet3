using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNet3.AppLib.StartupExt
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {

        /*
         * Once an authentication cookie is created, it becomes the single source of identity. 
         * If a user account is invalidated and not valid anymore in backend, the app's cookie 
         * authentication system continues to process requests based on the authentication cookie.
         * 
         * The user remains signed into the app as long as the authentication cookie is valid.
         * The ValidatePrincipal event can be used to intercept and override validation of the 
         * cookie identity. Validating the cookie on every request mitigates the risk of revoked 
         * users accessing the app.
         * 
         */

        /// <summary>
        /// ValidatePrincipal method runs in every request.
        /// </summary>
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            try
            {
                if (context.HttpContext.Session.IsAvailable)
                {
                    ClaimsPrincipal user = context.Principal;

                    bool login = context.HttpContext.Session.GetKey<bool>("login");

                    // Check other params if available.

                    if (!(user.Identity.IsAuthenticated && login))
                    {
                        context.RejectPrincipal();
                        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                }
                else
                {
                    context.RejectPrincipal();
                }
            }
            catch
            {
                // Session has not been configured yet!
            }
        }
    }

    

    /// <summary>
    /// Authentication is the process to validate an anonymous user based on some credentials
    /// </summary>
    public static class AuthenticationExtension
    {
        public static IServiceCollection _AddAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Cookie.Name = "__app__auth__";
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ClaimsIssuer = "__app__";
                    options.ReturnUrlParameter = "ReturnUrl";
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true; // false makes xss vulnerability
                    options.ExpireTimeSpan = TimeSpan.FromSeconds(1200);
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Validate();
                    options.EventsType = typeof(CustomCookieAuthenticationEvents);

                    //options.Events.OnRedirectToLogin = (context) =>
                    //{
                    //    // Ajax Request:
                    //    // context.Response.Headers["Location"] = context.RedirectUri;
                    //    // context.Response.StatusCode = 401;
                    //    // context.Response.Redirect(context.RedirectUri);
                    //    return Task.CompletedTask;
                    //};
                    //options.Events.OnRedirectToLogout = (context) =>
                    //{
                    //    return Task.CompletedTask;
                    //};
                    //options.Events.OnRedirectToAccessDenied = (context) =>
                    //{
                    //    return Task.CompletedTask;
                    //};
                    //options.Events.OnSignedIn = (context) =>
                    //{
                    //    return Task.CompletedTask;
                    //};
                })
            ;

            services.AddScoped<CustomCookieAuthenticationEvents>();

            return services;
        }

        /// <summary>
        /// Authentication and Authorization must be in between Routing and Endpoints.
        /// </summary>
        public static IApplicationBuilder _UseAuthentication(this IApplicationBuilder app)
        {
            //app.UseCookiePolicy(new CookiePolicyOptions()
            //{
            //    MinimumSameSitePolicy = SameSiteMode.Lax
            //});

            app.UseAuthentication();

            return app;
        }
    }
}
