using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;

namespace SpyderByteAPI.DataAccess
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
