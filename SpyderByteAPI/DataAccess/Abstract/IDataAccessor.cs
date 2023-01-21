using SpyderByteAPI.Models.Abstract;

namespace SpyderByteAPI.DataAccess.Abstract
{
    public interface IDataAccessor<T>
    {
        IDataResponse<IQueryable<T>> Get();

        IDataResponse<T?> Get(int id);

        IDataResponse<T?> Post(T insertObject);

        IDataResponse<T?> Put(int id, T updateObject);

        IDataResponse<T?> Patch(int id, IPatchable patchObject);

        IDataResponse<T?> Delete(int id);

        //IDataResponse<T?> Option(int id);

        //Head
    }
}
