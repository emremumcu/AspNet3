using AspNet3.AppLib.Abstract;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNet3.AppLib.Concrete
{
    public class MockAuthorize : IAuthorize
    {
        private const string mockScheme = "MockScheme";

        public List<Claim> AuthorizeUser(string userid)
        {
            List<Claim> userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, "nameid"),
                new Claim(ClaimTypes.Name, "name"),
                new Claim(ClaimTypes.Role, "role")
            };

            return userClaims;
        }

        public ClaimsPrincipal GetClaimsPrincipal()
        {
            List<Claim> mockClaims = new List<Claim>() 
            {
                new Claim(ClaimTypes.NameIdentifier, "MockNameIdentifier"),
                new Claim(ClaimTypes.Name, "MockName")
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(mockClaims, mockScheme);

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return claimsPrincipal;
        }

        public AuthenticationProperties GetAuthenticationProperties()
        {
            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                IsPersistent = true,
                IssuedUtc = DateTimeOffset.UtcNow,
                RedirectUri = "RedirectUri"
            };

            return authProperties;
        }
    }
}
