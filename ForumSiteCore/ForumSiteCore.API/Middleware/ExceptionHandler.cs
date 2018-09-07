using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ForumSiteCore.Business.Responses;

namespace ForumSiteCore.API.Middleware
{
    // most of the credit for this goes to this: https://stackoverflow.com/a/48625298
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            Log.Error(exception, "An error occurred");
            await response.WriteAsync(JsonConvert.SerializeObject(new FiveHundredErrorResponse() { Data = exception, Message = "An error occurred", Status = "error" }));
        }
    }
}
