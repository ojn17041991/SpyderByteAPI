using SpyderByteAPI.Enums;

namespace SpyderByteAPI.DataAccess.Abstract
{
    public interface IDataResponse<T>
    {
        T Data { get; }

        ModelResult Result { get; }
    }
}
