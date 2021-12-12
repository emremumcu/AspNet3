using AspNet3.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AspNet3.AppLib.Filters
{
    /*
     * ExceptionFilterAttribute
     * ------------------------
     * An abstract filter that runs asynchronously after an action has thrown an Exception. 
     * Subclasses must override OnException(ExceptionContext) or OnExceptionAsync(ExceptionContext) but not both.
     */

    public class WebExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public WebExceptionFilter(IWebHostEnvironment hostEnvironment, IModelMetadataProvider modelMetadataProvider)
        {
            _hostEnvironment = hostEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
        }

        public override void OnException(ExceptionContext context)
        {
            HttpStatusCode statusCode = (context.Exception as WebException != null && ((HttpWebResponse)(context.Exception as WebException).Response) != null)
                ? ((HttpWebResponse)(context.Exception as WebException).Response).StatusCode
                : GetStatusCode(context.Exception.GetType());

            if (_hostEnvironment.IsDevelopment())
            {
                // Detailed exception message
                ReturnContent(context, statusCode);
            }
            else
            {
                // Simple exception message
                ReturnView(context, statusCode);
            }
        }

        private void ReturnContent(ExceptionContext context, HttpStatusCode statusCode)
        {
            context.ExceptionHandled = true;

            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            var methodDescriptor = string.Format("{0}.{1}.{2}", controllerActionDescriptor.MethodInfo.ReflectedType.Namespace, controllerActionDescriptor.MethodInfo.ReflectedType.Name, controllerActionDescriptor.MethodInfo.Name);

            var controllerName = context.RouteData.Values["controller"];

            var actionName = context.RouteData.Values["action"];

            HttpResponse response = context.HttpContext.Response;

            response.StatusCode = (int)statusCode;
            
            response.ContentType = "application/json";

            var resultJ = JsonConvert.SerializeObject(
                new
                {
                    MethodInfo = methodDescriptor,
                    ControllerName = controllerName,
                    actionName = actionName,
                    message = "An exception has occured!",
                    isError = true,
                    errCode = statusCode,
                    errMessage = context.Exception.Message,
                    errTrace = context.Exception.StackTrace
                }, Formatting.Indented);

            response.ContentLength = resultJ.Length;

            response.WriteAsync(resultJ);
        }

        private void ReturnView(ExceptionContext context, HttpStatusCode statusCode)
        {
            context.ExceptionHandled = true;

            ViewResult result = new ViewResult { ViewName = "_Error" }; // (Views/Shared/_Error.cshtml)

            WebExceptionViewModel viewmodel = new WebExceptionViewModel() { 
                StatusCode = (int)statusCode,
                Message = context.Exception.Message
            };

            IModelMetadataProvider mmp = _modelMetadataProvider ?? new EmptyModelMetadataProvider();

            ViewDataDictionary vdd = new ViewDataDictionary(mmp, context.ModelState) { 
                Model = viewmodel
            };
            
            result.ViewData = vdd;
            
            result.ViewData.Add("EventTime", DateTime.Now);            

            context.Result = result;
        }
 
        public enum Exceptions
        {
            NullReferenceException = 1,
            FileNotFoundException = 2,
            OverflowException = 3,
            OutOfMemoryException = 4,
            InvalidCastException = 5,
            ObjectDisposedException = 6,
            UnauthorizedAccessException = 7,
            NotImplementedException = 8,
            NotSupportedException = 9,
            InvalidOperationException = 10,
            TimeoutException = 11,
            ArgumentException = 12,
            FormatException = 13,
            StackOverflowException = 14,
            SqlException = 15,
            IndexOutOfRangeException = 16,
            IOException = 17
        }

        private HttpStatusCode GetStatusCode(Type exceptionType)
        {
            Exceptions tryParseResult;

            if (Enum.TryParse<Exceptions>(exceptionType.Name, out tryParseResult))
            {
                switch (tryParseResult)
                {
                    case Exceptions.NullReferenceException: return HttpStatusCode.LengthRequired;
                    case Exceptions.FileNotFoundException: return HttpStatusCode.NotFound;
                    case Exceptions.OverflowException: return HttpStatusCode.RequestedRangeNotSatisfiable;
                    case Exceptions.OutOfMemoryException: return HttpStatusCode.ExpectationFailed;
                    case Exceptions.InvalidCastException: return HttpStatusCode.PreconditionFailed;
                    case Exceptions.ObjectDisposedException: return HttpStatusCode.Gone; 
                    case Exceptions.UnauthorizedAccessException: return HttpStatusCode.Unauthorized;
                    case Exceptions.NotImplementedException: return HttpStatusCode.NotImplemented; 
                    case Exceptions.NotSupportedException: return HttpStatusCode.NotAcceptable;
                    case Exceptions.InvalidOperationException: return HttpStatusCode.MethodNotAllowed;
                    case Exceptions.TimeoutException: return HttpStatusCode.RequestTimeout;
                    case Exceptions.ArgumentException: return HttpStatusCode.BadRequest;
                    case Exceptions.StackOverflowException: return HttpStatusCode.RequestedRangeNotSatisfiable;
                    case Exceptions.FormatException: return HttpStatusCode.UnsupportedMediaType;
                    case Exceptions.IOException: return HttpStatusCode.NotFound;
                    case Exceptions.IndexOutOfRangeException: return HttpStatusCode.ExpectationFailed;
                    default: return HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
