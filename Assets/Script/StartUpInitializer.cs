using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ResponseDtos;

public static class StartUpInitializer
{
    private static IServiceProvider serviceProvider;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        var serviceCollection = new ServiceCollection();

        // Add HttpClientFactory
        serviceCollection.AddHttpClient();

        serviceCollection.AddScoped<ExternalService>();

        serviceCollection.AddSingleton<IEqualityComparer<TopicResponseDto>>(provider =>
            new EqualityComparer<TopicResponseDto, string>(x => x.Id));

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public static IServiceProvider GetProvider => serviceProvider;
}

public class EqualityComparer<TEntity, TKey> : IEqualityComparer<TEntity> where TEntity : class
{
    private readonly Func<TEntity, TKey> _keySelector;

    public EqualityComparer(Func<TEntity, TKey> keySelector)
    {
        _keySelector = keySelector;
    }

    public bool Equals(TEntity x, TEntity y)
    {
        if (x == null || y == null)
            return false;
        return EqualityComparer<TKey>.Default.Equals(_keySelector(x), _keySelector(y));
    }

    public int GetHashCode(TEntity obj)
    {
        if (obj == null)
            return 0;
        return EqualityComparer<TKey>.Default.GetHashCode(_keySelector(obj));
    }
}
