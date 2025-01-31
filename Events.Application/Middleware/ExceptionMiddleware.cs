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
                    details = errorMessages,
                    occuredAt = ex.StackTrace
                };

                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse, Formatting.Indented));
            }
            catch (IncorrectDataException ex)
            {
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                var errorMessages = GetErrorMessages(ex);

                var errorResponse = new
                {
                    message = "Bad request.",
                    details = errorMessages,
                    occuredAt = ex.StackTrace
                };

                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse, Formatting.Indented));
            }
            catch (Exception ex)
            {
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;


                var errorResponse = new
                {
                    message = "An internal error occurred.",
                    details = GetErrorMessages(ex),
                    occuredAt = ex.StackTrace
                };

                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse, Formatting.Indented));
            }
        }

        static private List<string> GetErrorMessages(Exception ex)
        {
            var messages = new List<string>();
            while (ex != null)
            {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }
            return messages;
        }
    }
}

