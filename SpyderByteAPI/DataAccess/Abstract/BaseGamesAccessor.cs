using SpyderByteAPI.Enums;
using SpyderByteAPI.Models;
using SpyderByteAPI.Models.Abstract;
using System.Reflection;

namespace SpyderByteAPI.DataAccess.Abstract
{
    public abstract class BaseGamesAccessor<T> : IDataAccessor<IGame> where T : Game
    {
        protected readonly ApplicationDbContext context;
        protected abstract IQueryable<IGame> getAll();
        protected abstract IGame? getSingle(int id);
        protected abstract void addSingle(IGame addedGame);
        protected abstract void updateSingle(IGame originalGame, IGame updatedGame);
        protected abstract void deleteSingle(IGame deletedgame);



        public BaseGamesAccessor(ApplicationDbContext context)
        {
            this.context = context;
        }



        /// <summary>
        /// Gets a list of all games.
        /// </summary>
        public IDataResponse<IQueryable<IGame>> Get()
        {
            IQueryable<IGame> games = getAll();

            return new DataResponse<IQueryable<IGame>>(games, ModelResult.OK);
        }

        /// <summary>
        /// Gets a single game by ID.
        /// </summary>
        public IDataResponse<IGame?> Get(int id)
        {
            IGame? game = getSingle(id);

            if (game == null)
            {
                // Couldn't find an item of this ID.
                return new DataResponse<IGame?>(game, ModelResult.NotFound);
            }

            // Found the requested item.
            return new DataResponse<IGame?>(game, ModelResult.OK);
        }

        /// <summary>
        /// Inserts a new game.
        /// </summary>
        public IDataResponse<IGame?> Post(IGame insertObject)
        {
            if (insertObject.Id != null)
            {
                // ID is an identity field. You are not allowed to set it manually.
                return new DataResponse<IGame?>(null, ModelResult.IDGivenForIdentityField);
            }

            // Perform the post operation.
            addSingle(insertObject);
            context.SaveChanges();

            // The object was created.
            return new DataResponse<IGame?>(insertObject, ModelResult.Created);
        }

        /// <summary>
        /// Updates an existing game.
        /// </summary>
        public IDataResponse<IGame?> Put(int id, IGame updateObject)
        {
            if ((updateObject.Id ?? id) != id)
            {
                // The ID doesn't match the ID in the object.
                return new DataResponse<IGame?>(null, ModelResult.IDMismatchInPut);
            }

            IGame? game = getSingle(id);
            if (game == null)
            {
                // Couldn't find an item of this ID.
                return new DataResponse<IGame?>(game, ModelResult.NotFound);
            }

            if (updateObject.Id == null)
            {
                // The ID is optional, but we need to provide it for EF if it wasn't provided by the client.
                updateObject.Id = id;
            }

            // Perform the put operation.
            updateSingle(game, updateObject);
            context.SaveChanges();

            // The object was updated.
            return new DataResponse<IGame?>(updateObject, ModelResult.OK);
        }

        public IDataResponse<IGame?> Patch(int id, IGame patchObject)
        {
            IGame? patchGame = (T)patchObject;

            if (patchGame?.Id != null)
            {
                // The patchable object includes an ID.
                return new DataResponse<IGame?>(null, ModelResult.IDFoundInPatch);
            }

            IGame? game = getSingle(id);
            if (game == null)
            {
                // Couldn't find an item of this ID.
                return new DataResponse<IGame?>(game, ModelResult.NotFound);
            }

            // Go through each property, find those provided by the patch object, and update them.
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                object? propertyValue = property.GetValue(patchGame, null);
                if (propertyValue != null)
                {
                    game?.GetType()?.GetProperty(property.Name)?.SetValue(game, propertyValue);
                }
            }

            // Perform the patch operation.
            context.SaveChanges();

            // The object was patched.
            return new DataResponse<IGame?>(game, ModelResult.OK);
        }

        /// <summary>
        /// Deletes an existing game.
        /// </summary>
        public IDataResponse<IGame?> Delete(int id)
        {
            IGame? game = getSingle(id);
            if (game == null)
            {
                // Couldn't find an item of this ID.
                return new DataResponse<IGame?>(game, ModelResult.NotFound);
            }

            // Perform the delete operation.
            deleteSingle(game);
            context.SaveChanges();

            // The object was deleted.
            return new DataResponse<IGame?>(game, ModelResult.OK);
        }
    }
}
