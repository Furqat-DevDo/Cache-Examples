The IDistributedCache interface provides the following methods to manipulate items in the distributed cache implementation:

**Get, GetAsync:** Accepts a string key and retrieves a cached item as a byte[] array if found in the cache.
**Set, SetAsync:** Adds an item (as byte[] array) to the cache using a string key.
**Refresh, RefreshAsync:** Refreshes an item in the cache based on its key, resetting its sliding expiration timeout (if any).
**Remove, RemoveAsync:** Removes a cache item based on its string key.