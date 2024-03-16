using SpyderByteResources.Enums;
using SpyderByteResources.Responses.Abstract;

namespace SpyderByteResources.Responses
{
    public class DataResponse<T> : IDataResponse<T>
    {
        public DataResponse(T data, ModelResult result)
        {
            Data = data;
            Result = result;
        }

        public T Data { get; }

        public ModelResult Result { get; }
    }
}
