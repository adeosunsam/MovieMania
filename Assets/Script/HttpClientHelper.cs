using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using static StartUpInitializer;

public static class HttpClientHelper
{
    private static readonly HttpClient client;
    private static readonly string baseUrl;

    static HttpClientHelper()
    {
        IHttpClientFactory httpClientFactory = GetProvider.GetRequiredService<IHttpClientFactory>();
        client = httpClientFactory.CreateClient();
        baseUrl = "https://localhost:7153/api";
        //baseUrl = "https://leaderboard-o33d.onrender.com/api/Movie";
        //baseUrl = "https://odemwingie-001-site1.ktempurl.com/api";
    }

    public static async Task<T> GetAsync<T>(string path)
    {
        try
        {
            string url = $"{baseUrl}/{path}";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            T result = JsonConvert.DeserializeObject<T>(responseBody); //JsonUtility.FromJson<T>(responseBody);
            return result;
        }
        catch (HttpRequestException e)
        {
            Debug.LogError($"Request error: {e.Message}");
            return default;
        }
    }

    public static async Task<T> PostAsync<T>(string path, HttpContent content)
    {
        try
        {
            string url = $"{baseUrl}/{path}";
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            T result = JsonConvert.DeserializeObject<T>(responseBody);
            return result;
        }
        catch (HttpRequestException e)
        {
            Debug.LogError($"Request error: {e.Message}");
            return default;
        }
    }

    public static async Task PostAsync(string path, HttpContent content)
    {
        try
        {
            string url = $"{baseUrl}/{path}";
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Debug.LogError($"Request error: {e.Message}");
        }
    }
}
