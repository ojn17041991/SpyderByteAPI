using Microsoft.EntityFrameworkCore.Storage;
using SpyderByteDataAccess.Contexts;
using SpyderByteDataAccess.Transactions.Factories.Abstract;

namespace SpyderByteDataAccess.Transactions.Factories
{
    public class TransactionFactory : ITransactionFactory
    {
        private readonly ApplicationDbContext context;

        public TransactionFactory(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IDbContextTransaction> CreateAsync()
        {
            return await context.Database.BeginTransactionAsync();
        }
    }
}
