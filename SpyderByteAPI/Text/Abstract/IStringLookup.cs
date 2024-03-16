namespace SpyderByteAPI.Text.Abstract
{
    public interface IStringLookup<T>
    {
        string GetResource(T type);
    }
}
