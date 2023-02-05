using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Models;
using SpyderByteAPI.Models.Abstract;

namespace SpyderByteAPI.DataAccess
{
    public class WebGamesAccessor : BaseGamesAccessor<WebGame>
    {
        public WebGamesAccessor(ApplicationDbContext context) : base(context)
        {

        }

        protected override void addSingle(IGame addedGame)
        {
            throw new NotImplementedException();
        }

        protected override void deleteSingle(IGame deletedgame)
        {
            throw new NotImplementedException();
        }

        protected override IQueryable<IGame> getAll()
        {
            throw new NotImplementedException();
        }

        protected override IGame? getSingle(int id)
        {
            throw new NotImplementedException();
        }

        protected override void updateSingle(IGame originalGame, IGame updatedGame)
        {
            throw new NotImplementedException();
        }
    }
}
