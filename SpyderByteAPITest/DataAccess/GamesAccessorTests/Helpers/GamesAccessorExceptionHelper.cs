﻿using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.EntityFrameworkCore;
using SpyderByteDataAccess.Contexts;
using SpyderByteDataAccess.Accessors.Games;
using SpyderByteDataAccess.Paging.Factories.Abstract;

namespace SpyderByteTest.DataAccess.GamesAccessorTests.Helpers
{
    public class GamesAccessorExceptionHelper
    {
        public GamesAccessor Accessor;

        public GamesAccessorExceptionHelper()
        {
            // The DB Context is mocked out, so it will always throw an exception.
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
            var context = new Mock<ApplicationDbContext>(options);

            var pagedListFactory = new Mock<IPagedListFactory>();

            var logger = new Mock<ILogger<GamesAccessor>>();

            Accessor = new GamesAccessor(context.Object, pagedListFactory.Object, logger.Object);
        }
    }
}
