
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;

namespace AspNet3.ViewModels
{
    // User is a property of the Controller class which is why you cannot 'see' it from another class.

    /*
     * User is a property of the Controller class which is why you cannot 'see' it from another class.
     * You have a couple of options to get USER:
     * Pass the User value to your other class, for example:
     * public ctor(System.Security.Principal.IPrincipal user)
     * You can also retrieve the user from the HttpContext. The HttpContext class has a static method to get the current context. For example:
     * var user = HttpContext.Current.User;
     * BUT:
     * Accessing the current HTTP context from a separate class library is the type of messy architecture that ASP.NET Core tries to avoid.
     * There are a few ways to re-architect this in ASP.NET Core.
     * 
     * You can access the current HTTP context via the HttpContext property on any controller. The closest thing to your original code sample would be to pass HttpContext into the method you are calling
     * 
     * If you're writing custom middleware for the ASP.NET Core pipeline, the current request's HttpContext is passed into your Invoke method automatically
     * public Task Invoke(HttpContext context)
     * 
     * Finally, you can use the IHttpContextAccessor helper service to get the HTTP context in any class that is managed by the ASP.NET Core dependency injection system.
     * 
     * IHttpContextAccessor isn't always added to the service container by default, so register it in ConfigureServices just to be safe:
     * 
     * services.AddHttpContextAccessor();
    // if < .NET Core 2.2 use this
    //services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
     */

    public class TFAViewModel
    {

        public ClaimsIdentity User { get; set; }

        public String UserId
        {
            get {return User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value ?? throw new ArgumentException(nameof(ClaimTypes.NameIdentifier)); }
        }

        public String UserName
        {
            get { return User.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault()?.Value ?? throw new ArgumentException(nameof(ClaimTypes.Name)); }
        }

        public String RandomKey { get; set; }

        public String RandomKeyFormatted { get; set; }

        public String QRCodeData { get; set; }
    }
}
