public interface IRequestRouter
{
    Task<HttpResponseMessage?> SendRequestAsync(string who, string where, string what, string method, string token = null!);
}