using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ExternalService
{
    private static ExternalCallForData _external { get; set; }
    static ExternalService()
    {
        var provider = GameManager.Instance.Services.BuildServiceProvider();
        _external = provider.GetRequiredService<ExternalCallForData>();
    }


    public static async Task<List<TopicResponseDto>> FetchAvailableTopics(string userId)
    {
        try
        {
            var (IsSuccessful, message, data) = await _external.FetchAvailableTopics(userId);

            if (IsSuccessful && data != null)
            {
                return data.ToList();
            }
        }
        catch (Exception ex) { }

        return new List<TopicResponseDto>();
    }
}
