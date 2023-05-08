namespace SpyderByteAPI.Resources.Abstract
{
    public interface IStringLookup<T>
    {
        string GetResource(T type);
    }
}
