using Microsoft.AspNetCore.Http;
using Events.Application.Exceptions;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json;
namespace Events.Application.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ServiceException ex)
            {
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var errorMessages = GetErrorMessages(ex);

                var errorResponse = new
                {
                    message = "Service error. ",
                    details = errorMessages
                };

                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse,Formatting.Indented));
            }
            catch (Exception ex)
            {
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var errorResponse = new
                {
                    message = "An internal error occurred.",
                    details = ex.Message
                };

                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse, Formatting.Indented));
            }
        }

        private string GetErrorMessages(Exception ex)
        {
            var messages = new List<string>();
            while (ex != null)
            {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }
            return string.Join("\n", messages);
        }
    }
}

