using SpyderByteResources.Enums;

namespace SpyderByteResources.Models.Responses.Abstract
{
    public interface IDataResponse<T>
    {
        T Data { get; }

        ModelResult Result { get; }
    }
}
