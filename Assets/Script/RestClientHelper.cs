using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;


public class RestClientHelper
{
    public async Task<RestResponse> PostRequest<T>(string baseUrl, string resourcePath, T data, List<RequestHeaders> headers, bool excludeNull = false)
    {
        try
        {
            string json = string.Empty;
            if (data != null)
            {
                try
                {
                    if (excludeNull)
                    {
                        json = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                    }
                    else
                    {
                        json = JsonConvert.SerializeObject(data);
                    }
                }
                catch { }
            }

            var options = new RestClientOptions(baseUrl)
            {
                ThrowOnAnyError = false,
                MaxTimeout = 300000
            };

            var client = new RestClient(options);
            var request = new RestRequest(resourcePath, Method.Post);
            request.AddStringBody(json, ContentType.Json);
            request.AddParameter("application/json", json, ParameterType.RequestBody);

            string headersString = string.Empty;
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.key, header.value);
                }
                headersString = JsonConvert.SerializeObject(headers);
            }
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            var response = await client.ExecutePostAsync(request);

            return response;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<RestResponse> PutRequest<T>(string baseUrl, string resourcePath, T data, List<RequestHeaders> headers, bool excludeNull = false)
    {
        try
        {
            string json = string.Empty;
            if (data != null)
            {
                try
                {
                    if (excludeNull)
                    {
                        json = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                    }
                    else
                    {
                        json = JsonConvert.SerializeObject(data);
                    }
                }
                catch { }
            }

            var client = new RestClient(baseUrl);
            var request = new RestRequest(resourcePath, Method.Put);
            request.AddStringBody(json, ContentType.Json);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            string headersString = string.Empty;
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.key, header.value);
                }
                headersString = JsonConvert.SerializeObject(headers);
            }

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            var response = await client.ExecutePutAsync(request);

            return response;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<RestResponse> GetRequest<T>(string baseUrl, string resourcePath, List<RequestHeaders>? headers, string getBody = null)
    {
        try
        {
            var client = new RestClient(baseUrl);
            var request = new RestRequest(resourcePath, Method.Get);

            string headersString = string.Empty;
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.key, header.value);
                }
                headersString = JsonConvert.SerializeObject(headers);
            }
            if (getBody != null)
            {
                request.AddParameter("application/json", getBody, ParameterType.RequestBody);
            }


            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            var response = await client.ExecuteGetAsync(request);

            return response;
        }
        catch (Exception)
        {
            throw;
        }
    }
}

public class RequestHeaders
{
    public string key { get; set; }
    public string value { get; set; }
}
