using Microsoft.AspNetCore.Http;
using Events.Application.Exceptions;
using Newtonsoft.Json;
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
                    message = "Internal server error.",
                    details = errorMessages
                };

                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            }
            catch (IncorrectDataException ex)
            {
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                var errorMessages = GetErrorMessages(ex);

                var errorResponse = new
                {
                    message = "Bad request.",
                    details = errorMessages
                };

                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            }
            catch (Exception ex)
            {
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var errorMessages = GetErrorMessages(ex);
                var errorResponse = new
                {
                    message = "An internal error occurred.",
                    details = errorMessages
                };

                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse, Formatting.Indented));
            }
        }

        static private string GetErrorMessages(Exception ex)
        {
            var messages = new List<string>();
            while (ex != null)
            {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }
            return JsonConvert.SerializeObject(messages, Formatting.Indented);
        }
    }
}

