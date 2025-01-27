using Microsoft.AspNetCore.Mvc;
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
                var response = await _requestRouter.SendRequestAsync
                ("auth-service", "api/" + any, bodyContent, HttpContext.Request.Method);
                var answer = await response!.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return Content(answer);
                return BadRequest(answer);
            }
        }
    }
}
