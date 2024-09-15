using FluentAssertions;
using SpyderByteServices.Models.Authentication;
using SpyderByteTest.Services.PasswordServiceTests.Helpers;

namespace SpyderByteTest.Services.PasswordServiceTests
{
    public class IsPasswordValidTests
    {
        private readonly PasswordServiceHelper _helper;

        public IsPasswordValidTests()
        {
            _helper = new PasswordServiceHelper();
        }

        [Fact]
        public void Returns_True_If_Password_Is_Valid()
        {
            // Arrange
            PasswordVerification passwordVerification = new()
            {
                Password = "TESTPASSWORD",
                Hash = "QvPS3WTsuelDiXqKPgHMij9Ook0OKj/ZE7TVL5sB6VI=",
                Salt = "TESTSALT"
            };

            // Act
            bool isPasswordValid = _helper.Service.IsPasswordValid(passwordVerification);

            // Assert
            isPasswordValid.Should().BeTrue();
        }

        [Fact]
        public void Returns_False_If_Password_Is_Not_Valid()
        {
            // Arrange
            PasswordVerification passwordVerification = new()
            {
                Password = "TESTPASSWORD",
                Hash = "",
                Salt = "TESTSALT"
            };

            // Act
            bool isPasswordValid = _helper.Service.IsPasswordValid(passwordVerification);

            // Assert
            isPasswordValid.Should().BeFalse();
        }
    }
}
