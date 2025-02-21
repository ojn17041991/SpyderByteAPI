using FluentAssertions;
using FluentAssertions.Execution;
using SpyderByteTest.DataAccess.PagingTests.Extensions;
using SpyderByteTest.DataAccess.PagingTests.Factories.Helpers;
using System.Linq.Expressions;

namespace SpyderByteTest.DataAccess.PagingTests.Factories
{
    public class PagedListFactoryTests
    {
        private readonly PagedListFactoryHelper<int> _helper;

        public PagedListFactoryTests()
        {
            _helper = new PagedListFactoryHelper<int>();
        }

        [Fact]
        public async Task Can_Build_Paged_List_With_Filtering()
        {
            // Arrange.
            IList<int> collection = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            IQueryable<int> query = collection.ToAsyncQueryable();
            Expression<Func<int, bool>> filteringFunction =
                (i) =>
                    i < 5;
            Expression<Func<int, object>> orderingFunction =
                string.Empty switch
                {
                    _ => (i) => i
                };
            string? direction = "asc";
            int page = 1;
            int pageSize = 10;
            int expectedCount = 5;

            // Act.
            var pagedList = await _helper.Factory.BuildAsync(
                query,
                filteringFunction,
                orderingFunction,
                direction,
                page,
                pageSize
            );

            // Assert.
            using (new AssertionScope())
            {
                pagedList.Should().NotBeNull();
                pagedList.Items.Should().HaveCount(expectedCount);
                for (int i = 0; i < expectedCount; ++i)
                {
                    pagedList.Items[i].Should().Be(i);
                }
            }
        }

        [Fact]
        public async Task Can_Build_Paged_List_With_Ascending_Ordering()
        {
            // Arrange.
            IList<int> collection = new List<int>() { 5, 6, 7, 8, 9, 0, 1, 2, 3, 4 };
            IQueryable<int> query = collection.ToAsyncQueryable();
            Expression<Func<int, bool>> filteringFunction =
                (i) =>
                    true;
            Expression<Func<int, object>> orderingFunction =
                string.Empty switch
                {
                    _ => (i) => i
                };
            string? direction = "asc";
            int page = 1;
            int pageSize = 10;

            // Act.
            var pagedList = await _helper.Factory.BuildAsync(
                query,
                filteringFunction,
                orderingFunction,
                direction,
                page,
                pageSize
            );

            // Assert.
            using (new AssertionScope())
            {
                pagedList.Should().NotBeNull();
                pagedList.Items.Should().HaveCount(pageSize);
                for (int i = 0; i < pageSize; ++i)
                {
                    pagedList.Items[i].Should().Be(i);
                }
            }
        }

        [Fact]
        public async Task Can_Build_Paged_List_With_Descending_Ordering()
        {
            // Arrange.
            IList<int> collection = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            IQueryable<int> query = collection.ToAsyncQueryable();
            Expression<Func<int, bool>> filteringFunction =
                (i) =>
                    true;
            Expression<Func<int, object>> orderingFunction =
                string.Empty switch
                {
                    _ => (i) => i
                };
            string? direction = "desc";
            int page = 1;
            int pageSize = 10;

            // Act.
            var pagedList = await _helper.Factory.BuildAsync(
                query,
                filteringFunction,
                orderingFunction,
                direction,
                page,
                pageSize
            );

            // Assert.
            using (new AssertionScope())
            {
                pagedList.Should().NotBeNull();
                pagedList.Items.Should().HaveCount(pageSize);
                for (int i = 0; i < pageSize; ++i)
                {
                    pagedList.Items[i].Should().Be(pageSize - i - 1);
                }
            }
        }

        [Fact]
        public async Task Default_Ordering_Is_Ascending()
        {
            // Arrange.
            IList<int> collection = new List<int>() { 5, 6, 7, 8, 9, 0, 1, 2, 3, 4 };
            IQueryable<int> query = collection.ToAsyncQueryable();
            Expression<Func<int, bool>> filteringFunction =
                (i) =>
                    true;
            Expression<Func<int, object>> orderingFunction =
                string.Empty switch
                {
                    _ => (i) => i
                };
            string? direction = null;
            int page = 1;
            int pageSize = 10;

            // Act.
            var pagedList = await _helper.Factory.BuildAsync(
                query,
                filteringFunction,
                orderingFunction,
                direction,
                page,
                pageSize
            );

            // Assert.
            using (new AssertionScope())
            {
                pagedList.Should().NotBeNull();
                pagedList.Items.Should().HaveCount(pageSize);
                for (int i = 0; i < pageSize; ++i)
                {
                    pagedList.Items[i].Should().Be(i);
                }
            }
        }

        [Fact]
        public async Task Can_Build_Paged_List_At_Given_Page()
        {
            // Arrange.
            IList<int> collection = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            IQueryable<int> query = collection.ToAsyncQueryable();
            Expression<Func<int, bool>> filteringFunction =
                (i) =>
                    true;
            Expression<Func<int, object>> orderingFunction =
                string.Empty switch
                {
                    _ => (i) => i
                };
            string? direction = "asc";
            int page = 2;
            int pageSize = 5;

            // Act.
            var pagedList = await _helper.Factory.BuildAsync(
                query,
                filteringFunction,
                orderingFunction,
                direction,
                page,
                pageSize
            );

            // Assert.
            using (new AssertionScope())
            {
                pagedList.Should().NotBeNull();
                pagedList.Items.Should().HaveCount(pageSize);
                for (int i = 0; i < pageSize; ++i)
                {
                    pagedList.Items[i].Should().Be(pageSize + i);
                }
            }
        }

        [Fact]
        public async Task Page_Flags_Are_Correct_At_Start_Of_Paged_List()
        {
            // Arrange.
            IList<int> collection = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            IQueryable<int> query = collection.ToAsyncQueryable();
            Expression<Func<int, bool>> filteringFunction =
                (i) =>
                    true;
            Expression<Func<int, object>> orderingFunction =
                string.Empty switch
                {
                    _ => (i) => i
                };
            string? direction = "asc";
            int page = 1;
            int pageSize = 2;

            // Act.
            var pagedList = await _helper.Factory.BuildAsync(
                query,
                filteringFunction,
                orderingFunction,
                direction,
                page,
                pageSize
            );

            // Assert.
            using (new AssertionScope())
            {
                pagedList.Should().NotBeNull();
                pagedList.Page.Should().Be(page);
                pagedList.PageSize.Should().Be(pageSize);
                pagedList.HasPreviousPage.Should().Be(false);
                pagedList.HasNextPage.Should().Be(true);
            }
        }

        [Fact]
        public async Task Page_Flags_Are_Correct_In_Middle_Of_Paged_List()
        {
            // Arrange.
            IList<int> collection = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            IQueryable<int> query = collection.ToAsyncQueryable();
            Expression<Func<int, bool>> filteringFunction =
                (i) =>
                    true;
            Expression<Func<int, object>> orderingFunction =
                string.Empty switch
                {
                    _ => (i) => i
                };
            string? direction = "asc";
            int page = 3;
            int pageSize = 2;

            // Act.
            var pagedList = await _helper.Factory.BuildAsync(
                query,
                filteringFunction,
                orderingFunction,
                direction,
                page,
                pageSize
            );

            // Assert.
            using (new AssertionScope())
            {
                pagedList.Should().NotBeNull();
                pagedList.Page.Should().Be(page);
                pagedList.PageSize.Should().Be(pageSize);
                pagedList.HasPreviousPage.Should().Be(true);
                pagedList.HasNextPage.Should().Be(true);
            }
        }

        [Fact]
        public async Task Page_Flags_Are_Correct_At_End_Of_Paged_List()
        {
            // Arrange.
            IList<int> collection = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            IQueryable<int> query = collection.ToAsyncQueryable();
            Expression<Func<int, bool>> filteringFunction =
                (i) =>
                    true;
            Expression<Func<int, object>> orderingFunction =
                string.Empty switch
                {
                    _ => (i) => i
                };
            string? direction = "asc";
            int page = 5;
            int pageSize = 2;

            // Act.
            var pagedList = await _helper.Factory.BuildAsync(
                query,
                filteringFunction,
                orderingFunction,
                direction,
                page,
                pageSize
            );

            // Assert.
            using (new AssertionScope())
            {
                pagedList.Should().NotBeNull();
                pagedList.Page.Should().Be(page);
                pagedList.PageSize.Should().Be(pageSize);
                pagedList.HasPreviousPage.Should().Be(true);
                pagedList.HasNextPage.Should().Be(false);
            }
        }

        [Fact]
        public async Task Paged_List_Has_Correct_Count()
        {
            // Arrange.
            IList<int> collection = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            IQueryable<int> query = collection.ToAsyncQueryable();
            Expression<Func<int, bool>> filteringFunction =
                (i) =>
                    i < 5;
            Expression<Func<int, object>> orderingFunction =
                string.Empty switch
                {
                    _ => (i) => i
                };
            string? direction = "asc";
            int page = 5;
            int pageSize = 2;
            int expectedCount = 5;

            // Act.
            var pagedList = await _helper.Factory.BuildAsync(
                query,
                filteringFunction,
                orderingFunction,
                direction,
                page,
                pageSize
            );

            // Assert.
            using (new AssertionScope())
            {
                pagedList.Should().NotBeNull();
                pagedList.Count.Should().Be(expectedCount);
            }
        }
    }
}
