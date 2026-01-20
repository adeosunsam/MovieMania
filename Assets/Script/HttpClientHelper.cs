using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static class HttpClientHelper
{
    private static readonly HttpClient client;
    private const string baseUrl = "http://sammallos-001-site1.rtempurl.com";
    //private static readonly string baseUrl = "https://localhost:7153/api";
    private static readonly string authToken;

    static HttpClientHelper()
    {
        authToken = GameManager.Instance.token; // Replace with your actual token

        Debug.Log($"AUTH TOKEN: {authToken}");

        client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
        //client.DefaultRequestHeaders.Add("Authorization", $"Basic {authToken}");
    }

    public static async Task<T> GetAsync<T>(string path)
    {
        try
        {
            string url = $"{baseUrl}/{path}";
            Debug.Log($"URL PATH: {url}");

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            Debug.Log($"HEALTH CHECK: {responseBody}");

            T result = JsonConvert.DeserializeObject<T>(responseBody); //JsonUtility.FromJson<T>(responseBody);
            return result;
        }
        catch (HttpRequestException e)
        {
            Debug.LogError($"Request error: {path}: {e.Message}");
            return default;
        }
    }

    public static async Task<string> GetAsync(string path)
    {
        try
        {
            string url = $"{baseUrl}/{path}";

            Debug.Log($"URL PATH: {url}");

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
        catch (HttpRequestException e)
        {
            Debug.LogError($"Request error: {path}: {e.Message}");
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



//public class HttpClientHelper
//{
//    private const string baseUrl = "https://oluwakemi-001-site1.jtempurl.com/";
//    //private static readonly string baseUrl = "https://localhost:7153/api";

//    //public static async Task<T> GetAsync<T>(string path)
//    //{
//    //    try
//    //    {
//    //        string url = $"{baseUrl}/{path}";
//    //        HttpResponseMessage response = await client.GetAsync(url);
//    //        response.EnsureSuccessStatusCode();
//    //        string responseBody = await response.Content.ReadAsStringAsync();

//    //        T result = JsonConvert.DeserializeObject<T>(responseBody); //JsonUtility.FromJson<T>(responseBody);
//    //        return result;
//    //    }
//    //    catch (HttpRequestException e)
//    //    {
//    //        Debug.LogError($"Request error: {e.Message}");
//    //        return default;
//    //    }
//    //}

//    public static async Task<T> GetAsync<T>(string url) where T : class
//    {
//        try
//        {
//            using UnityWebRequest request = UnityWebRequest.Get(url);
//            UnityWebRequest response = await request.SendWebRequestAsync();
//            var data = JsonConvert.DeserializeObject<T>(response.downloadHandler.text);

//            return data;
//        }
//        catch (Exception)
//        {
//            return null;
//        }
//    }

//    public static async Task<T> PostAsync<T>(string path, object body)
//    {
//        try
//        {
//            string jsonBody = JsonConvert.SerializeObject(body);

//            using UnityWebRequest request = UnityWebRequest.Post($"{baseUrl}{path}", jsonBody, "application/json");
//            UnityWebRequest response = await request.SendWebRequestAsync();

//            var responseData = JsonConvert.DeserializeObject<T>(response.downloadHandler.text);

//            return responseData;
//        }
//        catch (Exception e)
//        {
//            Debug.LogError($"POST Request error: {e.Message}");
//            return default;
//        }
//    }

//    public static async Task PostAsync(string path, object body)
//    {
//        try
//        {
//            string jsonBody = JsonConvert.SerializeObject(body);

//            using UnityWebRequest request = UnityWebRequest.Post($"{baseUrl}{path}", jsonBody, "application/json");
//            UnityWebRequest response = await request.SendWebRequestAsync();
//        }
//        catch (Exception e)
//        {
//            Debug.LogError($"POST Request error: {e.Message}");
//        }
//    }
//}

//public static class UnityWebRequestExtensions
//{
//    public static Task<UnityWebRequest> SendWebRequestAsync(this UnityWebRequest request)
//    {
//        var tcs = new TaskCompletionSource<UnityWebRequest>();

//        request.SetRequestHeader("Authorization", $"Basic MTEyMzU4ODQ6NjAtZGF5ZnJlZXRyaWFs");

//        request.SendWebRequest().completed += _ =>
//        {
//            switch (request.result)
//            {
//                case UnityWebRequest.Result.ConnectionError:
//                case UnityWebRequest.Result.ProtocolError:
//                    //tcs.TrySetException(new Exception(request.error));
//                    tcs.TrySetResult(request);
//                    break;
//                default:
//                    tcs.TrySetResult(request);
//                    break;
//            }
//        };
//        return tcs.Task;
//    }
//}
