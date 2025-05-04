using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static RequestDtos;
using static ResponseDtos;
using static QuestionDto;
using System.Diagnostics;
using UnityEngine;

public class ExternalService
{
    public static async Task<UserDetailDto> GetUserById(string userId)
    {
        try
        {
            string path = $"api/Movie/user/{userId}";
            var data = await HttpClientHelper.GetAsync<GenericResponse<UserDetailDto>>(path);

            if (data != null)
            {
                return data.Data;
            }
        }
        catch (Exception) { }

        return new UserDetailDto();
    }

    public static async Task<List<UserDetailDto>> SearchUsers(string userId, string searchParam)
    {
        try
        {
            string path = $"api/Movie/user/{userId}/list?searchParam={searchParam}";
            var data = await HttpClientHelper.GetAsync<GenericResponse<List<UserDetailDto>>>(path);

            if (data != null)
            {
                return data.Data;
            }
        }
        catch (Exception) { }

        return new List<UserDetailDto>();
    }

    public static async Task<List<TopicResponseDto>> FetchAvailableTopics(string userId)
    {
        try
        {
            string path = $"api/Movie/topics/{userId}";
            var data = await HttpClientHelper.GetAsync<GenericResponse<ICollection<TopicResponseDto>>>(path);

            if (data != null)
            {
                return data.Data.ToList();
            }
        }
        catch (Exception) { }

        return new List<TopicResponseDto>();
    }

    public static async Task<bool> HeathCheck()
    {
        try
        {
            string path = $"health";
            var data = await HttpClientHelper.GetAsync(path);

            UnityEngine.Debug.Log($"HEALTH CHECK RESULT: {data}");

            if (data != null && (data.Equals("Healthy", StringComparison.OrdinalIgnoreCase)
                || data.Equals("Ok", StringComparison.OrdinalIgnoreCase)))
            {
                UnityEngine.Debug.Log($"HEALTH CHECK FLAG");
                return true;
            }
        }
        catch (Exception ex) { UnityEngine.Debug.Log($"EXCEPTION: {ex.Message}"); }

        return false;
    }

    public static async Task<UserGamingCountDto> FetchUserGamingCount(string userId)
    {
        try
        {
            var url = $"api/Movie/game-count/{userId}";
            var response = await HttpClientHelper.GetAsync<GenericResponse<UserGamingCountDto>>(url);
            if (response != null)
            {
                return response.Data;
            }
        }
        catch (Exception) { }

        return null;
    }

    public static async void Login(UserDetailDto request)
    {
        try
        {
            string jsonPostData = JsonConvert.SerializeObject(request);
            HttpContent content = new StringContent(jsonPostData, Encoding.UTF8, "application/json");

            await HttpClientHelper.PostAsync("api/Movie/create-user", content);
        }
        catch { }
    }

    public static async Task<bool> FollowTopic(string userId, string topicId)
    {
        try
        {
            HttpContent content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

            var response = await HttpClientHelper.PostAsync<GenericResponse<bool>>($"api/Movie/follow-topic/{topicId}/{userId}", content);

            if (response != null)
            {
                return response.Data;
            }
        }
        catch(Exception ex) {
            UnityEngine.Debug.Log("Error in FollowTopic: " + ex.Message);
        }
        return false;
    }

    public static async Task<ICollection<Question>> GetQuestionByTopic(string topicId)
    {
        try
        {
            string path = $"api/Movie/question/{topicId}";
            var data = await HttpClientHelper.GetAsync<GenericResponse<ICollection<Question>>>(path);

            if (data != null)
            {
                return data.Data.ToList();
            }
        }
        catch (Exception) { }

        return new List<Question>();
    }

    public static async Task<List<UserActivityResponse>> GetUserActivity(string userId)
    {
        try
        {
            string path = $"api/Movie/activity/{userId}";
            var data = await HttpClientHelper.GetAsync<GenericResponse<List<UserActivityResponse>>>(path);

            if (data != null)
            {
                return data.Data;
            }
        }
        catch (Exception) { }

        return new List<UserActivityResponse>();
    }

    public static async Task<List<UserDetailResponseDto>> GetUserFriends(string userId)
    {
        try
        {
            string path = $"api/Movie/friends/{userId}";
            var data = await HttpClientHelper.GetAsync<GenericResponse<ICollection<UserDetailResponseDto>>>(path);

            if (data != null)
            {
                return data.Data.ToList();
            }
        }
        catch (Exception) { }

        return null;
    }

    /*public static async Task<TokenResponse> Login(string userId)
    {
        try
        {
            HttpContent content = new StringContent(jsonPostData, Encoding.UTF8, "application/json");

            await HttpClientHelper.PostAsync("create-user", content);
        }
        catch { }
    }*/
}



/*public class ExternalService
{
    public static async Task<List<TopicResponseDto>> FetchAvailableTopics(string userId)
    {
        try
        {
            string path = $"Movie/topics/{userId}";
            var data = await HttpClientHelper.GetAsync<GenericResponse<ICollection<TopicResponseDto>>>(path);

            if (data != null)
            {
                return data.Data.ToList();
            }
        }
        catch (Exception) { }

        return new List<TopicResponseDto>();
    }

    //public static async Task<List<TopicResponseDto>> FetchAvailableTopics()
    //{
    //    _ = Task.Run(() =>
    //    {
    //        MainThreadDispatcher.Enqueue(async () =>
    //        {
    //            var availableTopic = await FetchAvailableTopics("");
    //        });
    //    });
    //}

    public static async Task<UserGamingCountDto> FetchUserGamingCount(string userId)
    {
        try
        {
            var url = $"Movie/game-count/{userId}";
            var response = await HttpClientHelper.GetAsync<GenericResponse<UserGamingCountDto>>(url);
            if (response != null)
            {
                return response.Data;
            }
        }
        catch (Exception) { }

        return null;
    }

    public static async void Login(UserDetailDto request)
    {
        try
        {
            string jsonPostData = JsonConvert.SerializeObject(request);
            HttpContent content = new StringContent(jsonPostData, Encoding.UTF8, "application/json");

            await HttpClientHelper.PostAsync("Movie/create-user", content);
        }
        catch { }
    }

    public static async Task<ICollection<Question>> GetQuestionByTopic(string topicId)
    {
        try
        {
            string path = $"Movie/question/{topicId}";
            var data = await HttpClientHelper.GetAsync<GenericResponse<ICollection<Question>>>(path);

            if (data != null)
            {
                return data.Data.ToList();
            }
        }
        catch (Exception) { }

        return new List<Question>();
    }

    public static async Task<List<UserActivityResponse>> GetUserActivity(string userId)
    {
        try
        {
            string path = $"Movie/activity/{userId}";
            var data = await HttpClientHelper.GetAsync<GenericResponse<List<UserActivityResponse>>>(path);

            if (data != null)
            {
                return data.Data;
            }
        }
        catch (Exception) { }

        return new List<UserActivityResponse>();
    }

    public static async Task<ICollection<UserDetailResponseDto>> GetUserFriends(string userId)
    {
        try
        {
            string path = $"Movie/friends/{userId}";
            var data = await HttpClientHelper.GetAsync<GenericResponse<ICollection<UserDetailResponseDto>>>(path);

            if (data != null)
            {
                return data.Data.ToList();
            }
        }
        catch (Exception) { }

        return new List<UserDetailResponseDto>();
    }

    //public static async Task<TokenResponse> Login(string userId)
    //{
    //    try
    //    {
    //        HttpContent content = new StringContent(jsonPostData, Encoding.UTF8, "application/json");

    //        await HttpClientHelper.PostAsync("create-user", content);
    //    }
    //    catch { }
    //}
}*/
