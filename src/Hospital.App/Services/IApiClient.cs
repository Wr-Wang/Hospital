using System.Threading.Tasks;

namespace Hospital.App.Services;

public interface IApiClient
{
    Task<TResponse> GetAsync<TResponse>(string route);
    Task<TResponse> PostAsync<TResponse>(string route, object payload);
}
