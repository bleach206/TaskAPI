namespace Common.Interface
{
    public interface IETagCache
    {
        T GetCachedObject<T>(string cacheKeyName);
        bool SetCachedObject<T>(string cacheKeyName, T objectToCache, byte[] rowVersion, int minutes);
    }
}
