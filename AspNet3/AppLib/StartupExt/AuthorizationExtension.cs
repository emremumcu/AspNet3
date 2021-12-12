using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet3.AppLib.StartupExt
{
    public static class AuthorizationPolicyLibrary
    {
        public static AuthorizationPolicy defaultPolicy = new AuthorizationPolicyBuilder()
           .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
           .RequireAuthenticatedUser()
           .Build();

        public static AuthorizationPolicy fallbackPolicy = new AuthorizationPolicyBuilder()
           .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
           .RequireAuthenticatedUser()
           .Build();

        public static AuthorizationPolicy adminPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .RequireRole("admin")
            .Build();

        public static AuthorizationPolicy age18Policy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            //.Requirements.Add(new AgeRequirement(18))
            .Build();

        public static AuthorizationPolicy assertionPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .RequireRole("admin")
            // The Require Assertion method takes a lambda that receives the Http Context object and returns a Boolean value. 
            // Therefore, the assertion is simply a conditional statement.
            .RequireAssertion(ctx => { return ctx.User.HasClaim("editor", "contents") || ctx.User.HasClaim("level", "senior"); })
            .Build();
    }

    public static class AuthorizationExtension
    {
        public static IServiceCollection _AddAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                /*
                 * Gets or sets the default authorization policy with no policy name specified.
                 * Defaults to require authenticated users.
                 * 
                 * The DefaultPolicy is the policy that is applied when:
                 *      (*) You specify that authorization is required, either using RequireAuthorization(), by applying an AuthorizeFilter, 
                 *          or by using the[Authorize] attribute on your actions / Razor Pages.
                 *      (*) You don't specify which policy to use.
                 *      
                 */
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                     .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
                     .RequireAuthenticatedUser()
                     .Build();

                /*
                 * Gets or sets the fallback authorization policy when no IAuthorizeData have been provided.
                 * By default the fallback policy is null.
                 * 
                 * The FallbackPolicy is applied when the following is true:
                 *      (*) The endpoint does not have any authorisation applied. No[Authorize] attribute, no RequireAuthorization, nothing.
                 *      (*) The endpoint does not have an[AllowAnonymous] applied, either explicitly or using conventions.
                 *      
                 * So the FallbackPolicy only applies if you don't apply any other sort of authorization policy, 
                 * including the DefaultPolicy, When that's true, the FallbackPolicy is used.
                 * By default, the FallbackPolicy is a no - op; it allows all requests without authorization.
                 * 
                 */
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                     .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
                     .RequireAuthenticatedUser()
                     .Build();


                options.AddPolicy("ClaimPolicy", policy => policy.RequireClaim("claim1"));
                options.AddPolicy("ClaimWithValuePolicy", policy => policy.RequireClaim("claim2", "val1", "val2"));
                options.AddPolicy("SingleRolePolicy", policy => policy.RequireRole("role1"));
                options.AddPolicy("MultiRolePolicy", policy => policy.RequireRole("role1", "role2"));
                options.AddPolicy("ClaimRoleCombinedPolicy", policy => { policy.RequireClaim("claim1"); policy.RequireRole("role1"); });
                options.AddPolicy("InlineDefinedPolicy", policy =>
                {
                    policy.AddAuthenticationSchemes("Cookie, Bearer");
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin");
                    policy.RequireClaim("editor", "contents");
                });
                options.AddPolicy("DefinedPolicy", AuthorizationPolicyLibrary.assertionPolicy);
                options.AddPolicy("PolictWithRequirement", policy => policy.Requirements.Add(new MinimumAgeRequirement(18)));
            });

            services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();

            return services;
        }


        /// <summary>
        /// UseRouting, UseAuthentication, UseAuthorization, and UseEndpoints must be called in the order of typing
        /// </summary>
        public static IApplicationBuilder _UseAuthorization(this IApplicationBuilder app)
        {
            app.UseAuthorization();

            return app;
        }
    }

    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public int Age { get; private set; }

        public MinimumAgeRequirement(int minimumAge)
        {
            Age = minimumAge;
        }
    }

    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            var user = context.User;

            if (!user.HasClaim(c => c.Type == "Age")) return Task.CompletedTask;

            var since = Convert.ToInt32(user.FindFirst("Age").Value);

            if (since >= requirement.Age) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

    public class PermissionHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var pendingRequirements = context.PendingRequirements.ToList();

            foreach (var requirement in pendingRequirements)
            {
                // ***
            }

            return Task.CompletedTask;
        }
    }
}
