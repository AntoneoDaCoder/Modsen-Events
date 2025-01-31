public interface IRequestRouter
{
    Task<HttpResponseMessage?> SendRequestAsync(string who, string where, HttpContent? what, string method, string? token = null,
        IEnumerable<KeyValuePair<string, string?[]>>? additionalHeaders = null);
}