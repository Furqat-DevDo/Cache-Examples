using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace RedisCache.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IDistributedCache distributedCache)
    {
        _logger = logger;
        _distributedCache = distributedCache;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("FromCache")]
    public async  Task<IEnumerable<WeatherForecast>> GetFromCache(string key)
    {
        var weathers = await  _distributedCache.GetRecordAsync<IEnumerable<WeatherForecast>>(key);
        if (weathers is not null) return weathers;
        
        await _distributedCache.SetRecordAsync(key, Get(), new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(20),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
        });
            
        return (await  _distributedCache.GetRecordAsync<IEnumerable<WeatherForecast>>(key))!;
    }
}
