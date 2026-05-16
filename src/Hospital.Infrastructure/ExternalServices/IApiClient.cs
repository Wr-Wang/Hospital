using System.Threading.Tasks;

namespace Hospital.Infrastructure.ExternalServices;

public interface IApiClient
{
    Task<TResponse> GetAsync<TResponse>(string route);
    Task<TResponse?> GetAsyncOrDefault<TResponse>(string route) where TResponse : class;
    Task<TResponse> PostAsync<TResponse>(string route, object payload);
    Task PutAsync(string route, object payload);
    Task PatchAsync(string route);
    Task DeleteAsync(string route);
}
