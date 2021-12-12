using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNet3.AppLib.Abstract
{
    public interface IAuthorize
    {
        public List<Claim> AuthorizeUser(string userid);
        public ClaimsPrincipal GetClaimsPrincipal();
        public AuthenticationProperties GetAuthenticationProperties();
    }
}
