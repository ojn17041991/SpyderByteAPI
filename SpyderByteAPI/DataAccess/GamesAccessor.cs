using Microsoft.EntityFrameworkCore;
using SpyderByteAPI.DataAccess.Abstract;
using SpyderByteAPI.Enums;
using SpyderByteAPI.Models;

namespace SpyderByteAPI.DataAccess
{
    public class GamesAccessor : IDataAccessor<Game>
    {
        private readonly ApplicationDbContext context;



        public GamesAccessor(ApplicationDbContext context)
        {
            this.context = context;
        }



        /// <summary>
        /// Gets a list of all games.
        /// </summary>
        public IDataResponse<IQueryable<Game>> Get()
        {
            return new DataResponse<IQueryable<Game>>(context.Games, ModelResult.OK);
        }

        /// <summary>
        /// Gets a single game by ID.
        /// </summary>
        public IDataResponse<Game?> Get(int id)
        {
            Game? game = context.Games.SingleOrDefault(g => g.Id == id);

            if (game == null)
            {
                // Couldn't find an item of this ID.
                return new DataResponse<Game?>(game, ModelResult.NotFound);
            }
            
            // Found the requested item.
            return new DataResponse<Game?>(game, ModelResult.OK);
        }

        /// <summary>
        /// Inserts a new game.
        /// </summary>
        public IDataResponse<Game?> Post(Game insertObject)
        {
            if (insertObject.Id != null)
            {
                // ID is an identity field. You are not allowed to set it manually.
                return new DataResponse<Game?>(null, ModelResult.IDGivenForIdentityField);
            }

            // Perform the post operation.
            context.Games.Add(insertObject);
            context.SaveChanges();

            // The object was created.
            return new DataResponse<Game?>(insertObject, ModelResult.Created);
        }

        /// <summary>
        /// Updates an existing game.
        /// </summary>
        public IDataResponse<Game?> Put(int id, Game updateObject)
        {
            if ((updateObject.Id ?? id) != id)
            {
                // The ID doesn't match the ID in the object.
                return new DataResponse<Game?>(null, ModelResult.IDMismatchInPut);
            }

            Game? game = context.Games.SingleOrDefault(g => g.Id == id);
            if (game == null)
            {
                // Couldn't find an item of this ID.
                return new DataResponse<Game?>(game, ModelResult.NotFound);
            }

            if (updateObject.Id == null)
            {
                // The ID is optional, but we need to provide it for EF if it wasn't provided by the client.
                updateObject.Id = id;
            }

            // Perform the put operation.
            context.Entry(game).CurrentValues.SetValues(updateObject);
            context.SaveChanges();

            // The object was updated.
            return new DataResponse<Game?>(updateObject, ModelResult.OK);
        }

        /// <summary>
        /// Deletes an existing game.
        /// </summary>
        public IDataResponse<Game?> Delete(int id)
        {
            Game? game = context.Games.SingleOrDefault(g => g.Id == id);
            if (game == null)
            {
                // Couldn't find an item of this ID.
                return new DataResponse<Game?>(game, ModelResult.NotFound);
            }

            // Perform the delete operation.
            context.Games.Remove(game);
            context.SaveChanges();

            // The object was deleted.
            return new DataResponse<Game?>(game, ModelResult.OK);
        }
    }
}
