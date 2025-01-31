using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
namespace Events.Gateway.Controllers
{
    [ApiController]
    [Route("api")]
    public class GatewayController : ControllerBase
    {
        private readonly IRequestRouter _requestRouter;
        public GatewayController(IRequestRouter router)
        {
            _requestRouter = router;
        }
        [Route("auth/{any:alpha}")]
        public async Task<IActionResult> HandleAuthRequest(string any)
        {
            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                var bodyContent = await reader.ReadToEndAsync();
                var serializedContent = new StringContent(bodyContent, Encoding.UTF8, "application/json");
                var response = await _requestRouter.SendRequestAsync
                ("auth-service", "api/" + any, serializedContent, HttpContext.Request.Method);
                var answer = await response!.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, answer);
            }
        }

        [Route("events/{**any}")]
        [Authorize(Policy = "AgeOver18")]
        public async Task<IActionResult> HandleDataRequest(string? any)
        {
            var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var contentType = Request.ContentType?.ToLower();
            HttpContent requestContent;
            IEnumerable<KeyValuePair<string, string?[]>>? optionalHeaders = null;
            if (contentType.StartsWith("multipart/form-data"))
            {
                var form = await Request.ReadFormAsync();
                var content = new MultipartFormDataContent();
                var file = form.Files[0];
                if (file == null)
                    return BadRequest("No file uploaded.");
                var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                var fileContent = new StreamContent(memoryStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "image/png");
                content.Add(fileContent, "Image", file.FileName);
                requestContent = content;
                if (Request.Headers.IfNoneMatch.Count > 0)
                    optionalHeaders = [new("If-None-Match", Request.Headers.IfNoneMatch.ToArray())];
            }
            else
            {
                using var reader = new StreamReader(Request.Body);
                var bodyContent = await reader.ReadToEndAsync();
                var content = new StringContent(bodyContent, Encoding.UTF8, "application/json");
                requestContent = content;
            }
            var queryString = Request.QueryString.Value;
            var response = await _requestRouter
                .SendRequestAsync("data-service", "api/" + any + queryString,
                requestContent,
                Request.Method, token: jwtToken, additionalHeaders: optionalHeaders);

            if (response.Content.Headers.ContentType?.MediaType == "image/png")
            {
                Response.Headers.CacheControl = response.Headers?.CacheControl?.ToString();
                Response.Headers.ETag = response.Headers?.ETag?.ToString();
                var responseBytes = await response.Content.ReadAsByteArrayAsync();
                return File(responseBytes, "image/png");
            }
            var answer = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, answer);
        }
    }

}
