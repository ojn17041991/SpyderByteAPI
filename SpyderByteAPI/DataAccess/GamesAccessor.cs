using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Models;
using SpyderByteAPI.Models.Abstract;

namespace SpyderByteAPI.DataAccess
{
    public class GamesAccessor : BaseGamesAccessor<Game>
    {
        public GamesAccessor(ApplicationDbContext context) : base(context)
        {

        }

        protected override IQueryable<IGame> getAll()
        {
            return context.Games;
        }

        protected override IGame? getSingle(int id)
        {
            return context.Games.SingleOrDefault(g => g.Id == id);
        }

        protected override void addSingle(IGame addedGame)
        {
            context.Games.Add((Game)addedGame);
        }

        protected override void updateSingle(IGame originalGame, IGame updatedGame)
        {
            context.Entry(originalGame).CurrentValues.SetValues(updatedGame);
        }

        protected override void deleteSingle(IGame deletedGame)
        {
            context.Games.Remove((Game)deletedGame);
        }
    }
}
