using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AspNet3.Controllers
{
    [AllowAnonymous]
    public class StatusCodePagesController : Controller
    {
        public IActionResult Index([Bind(Prefix = "id")] int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                HttpContext.Response.StatusCode = statusCode.Value;
            }

            IStatusCodeReExecuteFeature feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            StatusCodeReExecuteFeature reExecuteFeature = feature as StatusCodeReExecuteFeature;
            IExceptionHandlerFeature exception = HttpContext.Features.Get<IExceptionHandlerFeature>();
            IExceptionHandlerPathFeature exceptionPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            IStatusCodePagesFeature statusCodePagesFeature = HttpContext.Features.Get<IStatusCodePagesFeature>();

            string OriginalURL;

            if (feature != null)
            {
                OriginalURL =
                    feature?.OriginalPathBase
                    + feature?.OriginalPath
                    + feature?.OriginalQueryString;
            }

            return Content($"Status Code: {statusCode}");
        }
    }
}


