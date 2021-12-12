using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet3.AppLib.Extensions;
using AspNet3.AppLib.Filters;
using AspNet3.AppLib.Options;
using AspNet3.AppLib.StartupExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace AspNet3.Controllers
{
    [ServiceFilter(typeof(WebExceptionFilter))] // services.AddScoped<WebExceptionFilter>();
    public class HomeController : Controller
    {
        private readonly Config _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public HomeController(IHttpContextAccessor httpContextAccessor, IOptions<Config> config)
        {
            _config = config.Value;
            _httpContextAccessor = httpContextAccessor;

            _httpContextAccessor.HttpContext.Session.SetKey<DateTime>("LOGIN_DATETIME", DateTime.UtcNow); //  _session.SetKey<DateTime>("LOGIN_DATETIME", DateTime.UtcNow);            
        }


        private IMemoryCache _memCache;

        public IActionResult MemCache(IMemoryCache memCache)
        {
            _memCache = memCache;

            DateTime now;

            if (!_memCache.TryGetValue<DateTime>("CURRENT_DATETIME", out now))
                _memCache.Set<DateTime>("CURRENT_DATETIME", DateTime.Now, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(60)));

            now = _memCache.Get<DateTime>("CURRENT_DATETIME");

            return View();
        }



        [TempData]
        public string Message { get; set; }

        public IActionResult SampleTempData()
        {
            Message = $"TempData is set @ {DateTime.Now.ToString("yyyy.MM.dd HH:mm:sss")}";

            return View();
        }




        [Authorize]
        // Without arguments, [Authorize] attribute only checks that the user is authenticated (default policy => RequireAuthenticatedUser())
        public IActionResult Index() 
        {
            //ViewBag.IP1 = HttpContext.GetRemoteIPAddress().ToString();

            //ViewBag.IP2 = _accessor.HttpContext.Connection.RemoteIpAddress.ToString(); // sorunlu

            //ViewBag.IP3 = (Request.Headers.ContainsKey("X-Forwarded-For") ? (Request.Headers["X-Forwarded-For"].ToString()) : (_accessor.HttpContext.Connection.RemoteIpAddress.ToString()));



            string clientIP = HttpContext.GetRemoteIPAddress().ToString();


            return View();
        }

        public IActionResult UserInfo() => View();


        // The Roles property indicates that users in any of the listed roles would be granted access. 
        // To require multiple roles, you can apply the Authorize attribute multiple times, or write your own filter.
        // Roles are case-sensitive
        [Authorize(Roles = "admin, system")] // OR condition
        public IActionResult Action2() => Content("");


        // The AuthenticationSchemes property is a comma-separated string listing the authentication middleware components that the authorization layer will trust in the current context. 
        // In other words, it states that access to the Backoffice Controller class is allowed only if the user is authenticated through the Cookies scheme and has any of the listed roles.
        [Authorize(Roles = "admin, system", AuthenticationSchemes = "Cookie")]
        public IActionResult Action3() => Content("");


        // Users who belong to both admin and system roles have access
        [Authorize(Roles = "admin")]
        [Authorize(Roles = "system")] // AND condition
        public IActionResult Action4() => Content("");

    }
}
