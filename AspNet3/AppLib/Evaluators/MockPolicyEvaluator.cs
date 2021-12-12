using AspNet3.AppLib.Concrete;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNet3.AppLib.Evaluators
{
    // https://github.com/aspnet/Security/blob/master/src/Microsoft.AspNetCore.Authorization.Policy/PolicyEvaluator.cs
    public class MockPolicyEvaluator : IPolicyEvaluator
    {
        public virtual async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            try
            {
                if (context.Request.Path.Value.StartsWith("/Account")) return await Task.FromResult(AuthenticateResult.NoResult());

                MockAuthorize authorize = new MockAuthorize();                
                
                ClaimsPrincipal mockPrincipal = authorize.GetClaimsPrincipal();              

                context.User = mockPrincipal;                

                return await Task.FromResult(
                    AuthenticateResult.Success(
                        new AuthenticationTicket(mockPrincipal, authorize.GetAuthenticationProperties(), mockPrincipal.Identity.AuthenticationType)
                        )
                    );
            }
            catch (Exception ex)
            {
                //context.Response.Redirect("/Account/Login");
                return await Task.FromResult(AuthenticateResult.Fail(ex.Message));
            }
        }

        public virtual async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object resource)
        {
            if (authenticationResult.Succeeded) return await Task.FromResult(PolicyAuthorizationResult.Success());
            else return await Task.FromResult(PolicyAuthorizationResult.Challenge());
        }
    }
}
