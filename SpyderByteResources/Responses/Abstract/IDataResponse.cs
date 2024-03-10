using SpyderByteResources.Enums;

namespace SpyderByteResources.Responses.Abstract
{
    public interface IDataResponse<T>
    {
        T Data { get; }

        ModelResult Result { get; }
    }
}
