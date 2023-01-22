namespace SpyderByteAPI.Resources.Abstract
{
    public interface IStringLookup<T> : IStringResources
    {
        string GetResource(T type);
    }
}
