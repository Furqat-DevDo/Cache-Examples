using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;

namespace RedisCache;

public static class DistributedCacheExtensions
{
    public static async Task SetRecordAsync < T > (this IDistributedCache cache, string recordId, T data, DistributedCacheEntryOptions options = default) 
    {
        var jsonData = JsonSerializer.Serialize(data);
        await cache.SetStringAsync(recordId, jsonData, options);
    }
    public static async Task <T?> GetRecordAsync < T > (this IDistributedCache cache, string recordId) {
        var jsonData = await cache.GetStringAsync(recordId);
        if (jsonData is null) return default (T);
        
        return JsonSerializer.Deserialize < T > (jsonData);
    }
    
    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value)
    {
        return SetAsync(cache, key, value, new DistributedCacheEntryOptions());
    }
    
    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, GetJsonSerializerOptions()));
        return cache.SetAsync(key, bytes, options);
    }
    
    public static bool TryGetValue<T>(this IDistributedCache cache, string key, out T? value)
    {
        var val = cache.Get(key);
        value = default;
        if (val == null) return false;
        value = JsonSerializer.Deserialize<T>(val, GetJsonSerializerOptions());
        return true;
    }
    
    private static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions()
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }
}