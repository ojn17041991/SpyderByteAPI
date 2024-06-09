using FluentAssertions;
using SpyderByteTest.Services.PasswordServiceTests.Helpers;

namespace SpyderByteTest.Services.PasswordServiceTests
{
    public class GenerateNewHashTests
    {
        private readonly PasswordServiceHelper _helper;

        public GenerateNewHashTests()
        {
            _helper = new PasswordServiceHelper();
        }

        [Fact]
        public void Can_Generate_New_Hash()
        {
            // Arrange
            var password = "TESTPASSWORD";

            // Act
            var returnedHashData = _helper.Service.GenerateNewHash(password);

            // Assert
            returnedHashData.Hash.Should().NotBeEmpty();
            returnedHashData.Salt.Should().NotBeEmpty();
        }

        [Fact]
        public void Each_Hashing_Operation_Produces_A_Unique_Hash()
        {
            // Arrange
            var password = "HelloWorld123!";

            // Act
            var returnedHashData1 = _helper.Service.GenerateNewHash(password);
            var returnedHashData2 = _helper.Service.GenerateNewHash(password);

            // Assert
            returnedHashData1.Hash.Should().NotBe(returnedHashData2.Hash);
            returnedHashData1.Salt.Should().NotBe(returnedHashData2.Salt);
        }
    }
}
