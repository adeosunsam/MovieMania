using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ResponseDtos;
using static StartUpInitializer;

public static class PlayerPrefExtension<TEntity> where TEntity : class
{
    private static IEqualityComparer<TopicResponseDto> _comparer;

    static PlayerPrefExtension()
    {
        _comparer = GetProvider.GetRequiredService<IEqualityComparer<TopicResponseDto>>();
    }

    private const string Topics = "alltopics";
    /*private const string CoinKey = "coin";
    private const string ScoreKey = "score";
    private const string BombKey = "bomb";
    private const string StarKey = "star";
    private const string CurrentWeapon = "currentGun";*/

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
                    var savedData = Get(key) as List<TopicResponseDto>;
                    var collection = entity as List<TopicResponseDto>;
                    var updateTopics = collection.Except(savedData, _comparer);

                    Debug.LogWarning($"UpdatedTopics TO BE ADDED: {updateTopics.Count()}");
                    if (updateTopics == null || !updateTopics.Any())
                        return;

                    savedData.AddRange(updateTopics);

                    result = JsonConvert.SerializeObject(savedData);
                }
                break;
        }
        /*switch (key)
        {
            case StarKey:
                {
                    var collection = entity as List<LevelStar>;

                    List<LevelStar> levelStars = JsonConvert.DeserializeObject<List<LevelStar>>(getValue);

                    foreach (var level in collection)
                    {
                        var test = levelStars.Single(x => x.level == level.level);

                        //do not update if the new star count is less than the save star count
                        if (test.starCount >= level.starCount)
                        {
                            continue;
                        }

                        test.starCount = level.starCount;

                        //unlock next level if star is greater than 1
                        if (test.starCount < 2)
                        {
                            continue;
                        }

                        var nextLevel = levelStars.Single(x => x.level == Mathf.Clamp(level.level + 1, 1, levelStars.Count));

                        nextLevel.levelLocked = false;
                    }

                    result = JsonConvert.SerializeObject(levelStars);
                }
                break;
        }*/

        PlayerPrefs.SetString(key, result);

        PlayerPrefs.Save();
    }

    public static TEntity Get(string prefKey = null)
    {
        string key = prefKey ?? GetKey();
        if (PlayerPrefs.HasKey(key))
        {
            var result = PlayerPrefs.GetString(key);
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


