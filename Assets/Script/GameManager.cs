using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ServiceCollection Services { get; private set; }

    public static GameManager Instance;

    internal List<TopicResponseDto> TopicResponse { get; private set; }

    internal bool HasFetchedTopics;
    // Start is called before the first frame update
    private void Awake()
    {
        Services = new ServiceCollection();

        Services.AddScoped<ExternalCallForData>();
        Services.AddSingleton<RestClientHelper>();
        Services.AddScoped<ExternalService>();

        Instance = this;
    }
    /*void Start()
    {
        _= Task.Run(async () =>
        {
            TopicResponse =  await ExternalService.FetchAvailableTopics();
            Debug.LogWarning($"TopicResponse COunt:{TopicResponse.Count}"); 
            HasFetchedTopics = true;
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (TopicResponse != null && HasFetchedTopics)
        {
            Debug.LogWarning($"TopicResponse:{TopicResponse != null}, HasFetched: {HasFetchedTopics}");
            SharedResources.StartUp();
            HasFetchedTopics = false;
        }
    }*/
}
