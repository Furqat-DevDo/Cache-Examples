TrGetValue :
        
        CurrentDateTime = DateTime.Now;
    
        if (!_memoryCache.TryGetValue(CacheKeys.Entry, out DateTime cacheValue))
        {
            cacheValue = CurrentDateTime;
    
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(3));
    
            _memoryCache.Set(CacheKeys.Entry, cacheValue, cacheEntryOptions);
        }
    
        CacheCurrentDateTime = cacheValue;

Set :  

       _memoryCache.Set(CacheKeys.Entry, DateTime.Now, TimeSpan.FromDays(1));
       
GetOrCreate/Async :
        
        var cachedValue = _memoryCache.GetOrCreate(
                CacheKeys.Entry,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(3);
                    return DateTime.Now;
                });
        
        var cachedValue = await _memoryCache.GetOrCreateAsync(
                CacheKeys.Entry,
                cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(3);
                    return Task.FromResult(DateTime.Now);
                });
                
Get :   

        var cacheEntry = _memoryCache.Get<DateTime?>(CacheKeys.Entry);

With Cancellation Token :

    var cancellationTokenSource = new CancellationTokenSource();

    _memoryCache.Set(
        CacheKeys.DependentCancellationTokenSource,
        cancellationTokenSource);

    using var parentCacheEntry = _memoryCache.CreateEntry(CacheKeys.Parent);

    parentCacheEntry.Value = DateTime.Now;

    _memoryCache.Set(
        CacheKeys.Child,
        DateTime.Now,
        new CancellationChangeToken(cancellationTokenSource.Token));