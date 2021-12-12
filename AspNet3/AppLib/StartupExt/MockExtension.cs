using AspNet3.AppLib.Evaluators;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet3.AppLib.StartupExt
{
    public static class MockExtension
    {
        public static IServiceCollection _Mock(this IServiceCollection services, IWebHostEnvironment env, IConfiguration Configuration, IConfigurationRoot ConfigurationRoot)
        {
            if(env.IsDevelopment())
            {
                // INFO TestPolicyEvaluator
                // This logins user automatically (Mocking a real user in dev env)
                services.AddSingleton<IPolicyEvaluator, MockPolicyEvaluator>();
            }

            return services;
        }

        public static IApplicationBuilder _Mock(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
