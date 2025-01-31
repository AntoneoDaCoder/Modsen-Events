using System.Net.Http.Headers;
namespace Events.Application.Services
{

    public class GatewayRequestRouter : IRequestRouter
    {
        private readonly IHttpClientFactory _clientFactory;

        public GatewayRequestRouter(IHttpClientFactory factory)
        {
            _clientFactory = factory;
        }
        public async Task<HttpResponseMessage?> SendRequestAsync
            (string who, string where, HttpContent what, string method, string? token = null,
            IEnumerable<KeyValuePair<string, string?[]>>? additionalHeaders = null)
        {
            var client = _clientFactory.CreateClient(who);
            var msg = new HttpRequestMessage
            {
                Method = new HttpMethod(method),
                RequestUri = new Uri(where, UriKind.Relative),
                Content = what,
            };
            if (token != null)
                msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (additionalHeaders != null)
                foreach (var header in additionalHeaders)
                    msg.Headers.TryAddWithoutValidation(header.Key, header.Value);
            return await client.SendAsync(msg);
        }
    }
}
