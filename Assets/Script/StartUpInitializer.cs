using System;
using System.Collections.Generic;
using System.Reflection;
using Google;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;
using static ResponseDtos;

public static class StartUpInitializer
{
    private static IServiceProvider serviceProvider;
    private const string webClientId = "1079234788728-hn0tlk70vq570ap3ea4o4j3mqrn268sj.apps.googleusercontent.com";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true,
            RequestEmail = true,
            UseGameSignIn = false,
        };

        var serviceCollection = new ServiceCollection();

        // Add HttpClientFactory
        serviceCollection.AddHttpClient();

        serviceCollection.AddScoped<ExternalService>();

        serviceCollection.AddSingleton<IEqualityComparer<TopicResponseDto>, EqualityComparer<TopicResponseDto>>();

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public static IServiceProvider GetProvider => serviceProvider;
}

public class EqualityComparer<TEntity> : IEqualityComparer<TEntity> where TEntity : class
{
    public bool Equals(TEntity x, TEntity y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;

        foreach (var prop in typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var xValue = prop.GetValue(x);
            var yValue = prop.GetValue(y);

            if (!Equals(xValue, yValue))
                return false;
        }

        return true;
    }

    public int GetHashCode(TEntity obj)
    {
        if (obj is null) return 0;

        int hash = 17;
        foreach (var prop in typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var value = prop.GetValue(obj);
            hash = hash * 23 + (value?.GetHashCode() ?? 0);
        }

        return hash;
    }
}
