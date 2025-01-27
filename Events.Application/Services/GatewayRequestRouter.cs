using System.Text;
using System.Net.Http.Headers;
using System.Net;
namespace Events.Application.Services
{

    public class GatewayRequestRouter : IRequestRouter
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly HashSet<string> _validClients;
        private readonly Dictionary<string, HashSet<string>> _validClientActions;
        private readonly Dictionary<string, HashSet<string>> _validClientMethods;

        public GatewayRequestRouter(IHttpClientFactory factory)
        {
            _clientFactory = factory;
            _validClients = new HashSet<string>()
            {
                "auth-service",
            };
            _validClientActions = new Dictionary<string, HashSet<string>>()
            {
                {"auth-service", new() {"api/login", "api/register","api/refresh"} },
            };
            _validClientMethods = new Dictionary<string, HashSet<string>>()
            {
                {"auth-service", new() {"post"} },
            };
        }
        public async Task<HttpResponseMessage?> SendRequestAsync
            (string who, string where, string what, string method, string token = null!)
        {

            if (_validClients.Contains(who) &&
                _validClientActions[who].Contains(where) &&
                _validClientMethods[who].Contains(method.ToLower()))
            {
                var client = _clientFactory.CreateClient(who);
                var msg = new HttpRequestMessage
                {
                    Method = new HttpMethod(method),
                    RequestUri = new Uri(where, UriKind.Relative),
                    Content = new StringContent(what, Encoding.UTF8, "application/json")
                };

                if (token != null)
                    msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return await client.SendAsync(msg);
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Invalid client, method, or action specified.")
            };
        }
    }
}
