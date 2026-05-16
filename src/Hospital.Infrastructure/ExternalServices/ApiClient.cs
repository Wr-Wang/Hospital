using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Hospital.Infrastructure.ExternalServices;

public sealed class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TResponse> GetAsync<TResponse>(string route)
    {
        var response = await _httpClient.GetAsync(route);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>() ?? throw new InvalidOperationException("Response body was null.");
    }

    public async Task<TResponse?> GetAsyncOrDefault<TResponse>(string route) where TResponse : class
    {
        var response = await _httpClient.GetAsync(route);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<TResponse> PostAsync<TResponse>(string route, object payload)
    {
        var response = await _httpClient.PostAsJsonAsync(route, payload);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>() ?? throw new InvalidOperationException("Response body was null.");
    }

    public async Task PutAsync(string route, object payload)
    {
        var response = await _httpClient.PutAsJsonAsync(route, payload);
        response.EnsureSuccessStatusCode();
    }

    public async Task PatchAsync(string route)
    {
        var response = await _httpClient.PatchAsync(route, null);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(string route)
    {
        var response = await _httpClient.DeleteAsync(route);
        response.EnsureSuccessStatusCode();
    }
}
