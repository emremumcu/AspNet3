using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet3.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AspNet3.AppLib.StartupExt;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using AspNet3.AppLib.Tools;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AspNet3.Controllers
{   
    [AllowAnonymous]
    public class AccountController : Controller
    {
        [NonAction]
        private async Task<IActionResult> CustomLogin()
        {
            List<Claim> userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, "nameid"),
                new Claim(ClaimTypes.Name, "name"),
                new Claim(ClaimTypes.Role, "role")
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            AuthenticationProperties apr = new AuthenticationProperties() { ExpiresUtc = new DateTimeOffset().AddMinutes(20) };

            await HttpContext.RequestServices.GetRequiredService<IAuthenticationService>().SignInAsync(HttpContext, "cookies", claimsPrincipal, apr);

            HttpContext.Session.SetKey<bool>("login", true);

            HttpContext.User = claimsPrincipal;

            return RedirectToAction("Index", "Home");
        }

        [NonAction]
        private async Task<IActionResult> LoginUser(string username, string password, bool remember)
        {
            if (true) // validate user parameters
            {
                List<Claim> userClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, "nameid"),
                    new Claim(ClaimTypes.Name, "name"),
                    new Claim(ClaimTypes.Role, "role")
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                HttpContext.Session.SetKey<bool>("login", true);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(10),
                        IsPersistent = (true),
                        IssuedUtc = DateTime.UtcNow
                    }
                );

                // Check user identity:
                System.Security.Claims.ClaimsIdentity ci = ((System.Security.Claims.ClaimsIdentity)User.Identity);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                throw new Exception("Kullanıcı bilgileri hatalı.");
            }
        }

        [HttpGet]
        public IActionResult Login() => View(model: new LoginViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {            
            void ClearCaptchaText()
            {
                // model içindeki CaptchaCode temizlense bile, ModelState değeri de temizlenmeden CaptchaCode textbox değeri dolu geliyor
                model.Captcha.CaptchaCode = string.Empty;
                ModelState.Remove("Captcha.CaptchaCode"); // ModelState.SetModelValue("Captcha.CaptchaCode", new ValueProviderResult(string.Empty));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (!Captcha.ValidateCaptchaCode(model.Captcha.CaptchaCode, HttpContext))
                    {
                        ClearCaptchaText();
                        ModelState.AddModelError("Captcha", "Güvenlik kodu yanlış.");                        
                        return View(model);
                    }
                    else
                    {
                        return await LoginUser(model.Username, model.Password, model.RememberMe);
                    }
                }
                else
                {
                    ClearCaptchaText();
                    ModelState.AddModelError("ERR", $"Formda hatalar var. Lütfen hataları düzeltip, işleminizi yeniden deneyiniz.");                    
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ClearCaptchaText();
                ModelState.AddModelError("ERR", $"Hata: {ex.Message}");                
                return View(model);
            }
        }

        public IActionResult AccessDenied()
        {
            //return Forbid();
            //return StatusCode(403);
            return Content("You are not allowed to access this resource");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("login");

            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var cookie = this.Request.Cookies["__app__auth__"];

            if (cookie != null)
            {
                var options = new CookieOptions { Expires = DateTime.Now.AddDays(-1) };
                this.Response.Cookies.Append("__app__auth__", cookie, options);
            }

            return RedirectToAction("Login");
        }

        [Route("get-captcha-image")]
        public IActionResult GetCaptchaImage()
        {
            var result = Captcha.GenerateCaptchaImage(HttpContext);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }

        [HttpGet]
        public IActionResult Register() => View(model: new RegisterViewModel());

        // ASP.NET Core provides an attribute called FromServices to inject the dependencies directly into the controller’s action method
        // use FromServices attribute to inject the dependency directly (without constructor DI)
        // services.AddTransient<IAccountService, AccountService>(); is required !!!
        // So when we decorate the parameter with FromServices attribute, this instructs the ASP.NET 
        // Core to get it from the services container and inject the matching implementation. 
        // This makes the code cleaner and will also reduce the work of modifying the existing unit test cases.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {            
            void ClearCaptchaText()
            {
                // model içindeki CaptchaCode temizlense bile, ModelState değeri de temizlenmeden CaptchaCode textbox değeri dolu geliyor
                model.Captcha.CaptchaCode = string.Empty;
                ModelState.Remove("Captcha.CaptchaCode"); // ModelState.SetModelValue("Captcha.CaptchaCode", new ValueProviderResult(string.Empty));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (!Captcha.ValidateCaptchaCode(model.Captcha.CaptchaCode, HttpContext))
                    {
                        ClearCaptchaText();
                        ModelState.AddModelError("Captcha", "Güvenlik kodu yanlış.");                        
                        return View(model);
                    }
                    else
                    {
                        // INFO Register user here
                        await Task.Delay(1);
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    ClearCaptchaText();
                    ModelState.AddModelError("ERR", $"Formda hatalar var. Lütfen hataları düzeltip, işleminizi yeniden deneyiniz.");                    
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ClearCaptchaText();
                ModelState.AddModelError("ERR", $"Ex:{ex?.Message}{Environment.NewLine}InnerEx:{ex?.InnerException?.Message}");                
                return View(model);
            }
        }
    }
}

