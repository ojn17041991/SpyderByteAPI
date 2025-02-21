using Microsoft.EntityFrameworkCore.Storage;

namespace SpyderByteDataAccess.Transactions.Factories.Abstract
{
    public interface ITransactionFactory
    {
        Task<IDbContextTransaction> CreateAsync();
    }
}
