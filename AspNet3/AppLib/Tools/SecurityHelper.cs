using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNet3.AppLib.Tools
{
    // https://github.com/dotnet/aspnetcore/blob/master/src/Shared/SecurityHelper/SecurityHelper.cs
    public static class SecurityHelper
    {
        /// <summary>
        /// Add all ClaimsIdentities from an additional ClaimPrincipal to the ClaimsPrincipal
        /// Merges a new claims principal, placing all new identities first, and eliminating
        /// any empty unauthenticated identities from context.User
        /// </summary>
        /// <param name="existingPrincipal">The <see cref="ClaimsPrincipal"/> containing existing <see cref="ClaimsIdentity"/>.</param>
        /// <param name="additionalPrincipal">The <see cref="ClaimsPrincipal"/> containing <see cref="ClaimsIdentity"/> to be added.</param>        
        public static ClaimsPrincipal MergeUserPrincipal(ClaimsPrincipal existingPrincipal, ClaimsPrincipal additionalPrincipal)
        {
            var newPrincipal = new ClaimsPrincipal();

            // New principal identities go first
            if (additionalPrincipal != null)
            {
                newPrincipal.AddIdentities(additionalPrincipal.Identities);
            }

            // Then add any existing non empty or authenticated identities
            if (existingPrincipal != null)
            {
                newPrincipal.AddIdentities(existingPrincipal.Identities.Where(i => i.IsAuthenticated || i.Claims.Any()));
            }
            return newPrincipal;
        }

        //public static ClaimsPrincipal MergeUserPrincipal(ClaimsPrincipal? existingPrincipal, ClaimsPrincipal? additionalPrincipal)
    }
}
