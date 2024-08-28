using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static RequestDtos;
using static ResponseDtos;

public class ExternalService
{
    public static async Task<List<TopicResponseDto>> FetchAvailableTopics(string userId)
    {
        try
        {
            string path = $"topics/{userId}";
            var data = await HttpClientHelper.GetAsync<GenericResponse<ICollection<TopicResponseDto>>>(path);

            if (data != null)
            {
                return data.Data.ToList();
            }
        }
        catch (Exception) { }

        return new List<TopicResponseDto>();
    }

    public static async Task<UserGamingCountDto> FetchUserGamingCount(string userId)
    {
        try
        {
            var url = $"game-count/{userId}";
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

            await HttpClientHelper.PostAsync("create-user", content);
        }
        catch { }
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
