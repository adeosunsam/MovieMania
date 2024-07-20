using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ExternalCallForData
{
    public RestClientHelper _restClient { get; private set; }
    private const string baseUrl = "https://localhost:7153/api/Movie/";

    public ExternalCallForData()
    {
        var provider = GameManager.Instance.Services.BuildServiceProvider();
        _restClient = provider.GetRequiredService<RestClientHelper>();
    }

    public async Task<(bool IsSuccessful, string message, ICollection<TopicResponseDto> data)> FetchAvailableTopics(string userId)
    {
        try
        {
            string path = $"topics/{userId}";
            var headers = new List<RequestHeaders>();
            var response = await _restClient.GetRequest<GenericResponse<ICollection<TopicResponseDto>>>(baseUrl, path, headers);
            if (response != null && response.IsSuccessful)
            {
                if (string.IsNullOrEmpty(response.Content))
                {
                    return (false, response.ErrorMessage, null);
                }

                var result = JsonConvert.DeserializeObject<GenericResponse<ICollection<TopicResponseDto>>>(response.Content);

                return (true, "success", result.Data);
            }
            else
            {
                if (string.IsNullOrEmpty(response.ErrorMessage))
                {
                    var result = JsonConvert.DeserializeObject<GenericResponse<ICollection<TopicResponseDto>>>(response.Content);
                    return (false, result.ResponseMessage, new List<TopicResponseDto> { });
                }
                return (false, "No response from provider.", null);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<(bool IsSuccessful, string message, UserGamingCountDto data)> FetchUserGamingCount(string userId)
    {
        try
        {
            var headers = new List<RequestHeaders>();
            var url = $"game-count/{userId}";
            var response = await _restClient.GetRequest<GenericResponse<UserGamingCountDto>>(baseUrl, url, headers);
            if (response != null && response.IsSuccessful)
            {
                if (string.IsNullOrEmpty(response.Content))
                {
                    return (false, response.ErrorMessage, null);
                }

                var result = JsonConvert.DeserializeObject<GenericResponse<UserGamingCountDto>>(response.Content);

                return (true, "success", result.Data);
            }
            else
            {
                if (string.IsNullOrEmpty(response.ErrorMessage))
                {
                    var result = JsonConvert.DeserializeObject<GenericResponse<UserGamingCountDto>>(response.Content);
                    return (false, result.ResponseMessage, null);
                }
                return (false, "No response from provider.", null);
            }
        }
        catch (Exception ex)
        {
            return (false, "No response from provider.", null);
        }
    }

    public async void Login(UserDetailDto request)
    {
        try
        {
            var headers = new List<RequestHeaders>();
            var response = await _restClient.PostRequest(baseUrl, "create-user", request, headers);

            if (response != null && response.IsSuccessful)
            {
            }
        }
        catch { }
    }
}

public class TopicResponseDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Sprite Sprite { get; set; }
    public string Category { get; set; }
    public string Image { get; set; } // in base64
    public int QuestionCount { get; set; }
    public int FollowersCount { get; set; }
    public bool IsFollowed { get; set; }
}

public class UserDetailDto
{
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public Sprite Sprite { get; set; }
    public bool IsImageLoadingStopped { get; set; }
    public string Image { get; set; }
}

public class UserGamingCountDto
{
    public string Id { get; set; }
    public int TotalGamePlayed { get; set; }
}
