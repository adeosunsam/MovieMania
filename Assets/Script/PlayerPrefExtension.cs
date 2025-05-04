using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using UnityEngine;
using static ResponseDtos;

public static class PlayerPrefExtension<TEntity> where TEntity : class
{
    private static readonly IEqualityComparer<TopicResponseDto> _comparer;

    static PlayerPrefExtension()
    {
        _comparer = StartUpInitializer.GetProvider.GetRequiredService<IEqualityComparer<TopicResponseDto>>();
    }

    private const string Topics = "alltopics";

    public static void Add(TEntity entity, string prefKey = null)
    {
        string key = prefKey ?? GetKey();

        if (PlayerPrefs.HasKey(key))
        {
            return;
        }

        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        string stringValue = JsonConvert.SerializeObject(entity, settings);

        PlayerPrefs.SetString(key, stringValue);

        PlayerPrefs.Save();
    }

    public static bool HasKey(string prefKey = null)
    {
        string key = prefKey ?? GetKey();
        return PlayerPrefs.HasKey(key);
    }

    public static void UpdateDb(TEntity entity)
    {
        string key = GetKey();

        if (!PlayerPrefs.HasKey(key))
        {
            Add(entity, key);
            return;
        }
        string result = null;

        switch (key)
        {
            case Topics:
                {
                    var savedData = Get(key) as List<TopicResponseDto> ?? new List<TopicResponseDto>();
                    var collection = entity as List<TopicResponseDto>;
                    var updateTopics = collection.Except(savedData, _comparer);

                    Debug.LogWarning($"UpdatedTopics TO BE ADDED: {updateTopics.Count()}");
                    if (updateTopics == null || !updateTopics.Any())
                        return;

                    //Debug.Log($"SAVED PAYLOAD: {JsonConvert.SerializeObject(savedData, Formatting.Indented)}");
                    //Debug.Log($"INCOMING PAYLOAD: {JsonConvert.SerializeObject(collection, Formatting.Indented)}");
                    //Debug.Log($"UPDATED PAYLOAD: {JsonConvert.SerializeObject(updateTopics, Formatting.Indented)}");

                    savedData.RemoveAll(x => updateTopics.Select(y => y.Id).Contains(x.Id));

                    savedData.AddRange(updateTopics);

                    var settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };

                    result = JsonConvert.SerializeObject(savedData, settings);
                }
                break;
        }

        PlayerPrefs.SetString(key, result);

        PlayerPrefs.Save();
    }

    public static TEntity Get(string prefKey = null)
    {
        string key = prefKey ?? GetKey();
        if (PlayerPrefs.HasKey(key))
        {
            var result = PlayerPrefs.GetString(key);
            Debug.Log($"GET ENTITY: {JsonConvert.SerializeObject(result, Formatting.Indented)}");
            return JsonConvert.DeserializeObject<TEntity>(result);
        }
        return null;
    }

    public static void RemoveKey(string prefKey = null)
    {
        string key = prefKey ?? GetKey();
        PlayerPrefs.DeleteKey(key);
    }

    private static string GetKey()
    {
        string key = null;

        if (typeof(TEntity).FullName.Contains(nameof(TopicResponseDto)))
        {
            key = Topics;
        }

        /*if (typeof(TEntity).Name == nameof(Grenade))
        {
            key = BombKey;
        }

        else if (typeof(TEntity).Name == nameof(CurrentGun))
        {
            key = CurrentWeapon;
        }

        else if (typeof(TEntity).Name == nameof(Coin))
        {
            key = CoinKey;
        }

        else if (typeof(TEntity).Name == nameof(Score))
        {
            key = ScoreKey;
        }

        else if (typeof(TEntity).FullName.Contains(nameof(LevelStar)))
        {
            key = StarKey;
        }

        else if (typeof(TEntity).FullName.Contains(nameof(Gun))
            || typeof(TEntity).FullName.Contains(nameof(Weapon)))
        {
            key = WeaponKey;
        }*/

        return key;
    }
}


