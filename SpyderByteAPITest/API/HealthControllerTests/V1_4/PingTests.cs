using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace SpyderByteTest.API.HealthControllerTests.V1_4
{
    public class PingTests
    {
        [Fact]
        public async Task Can_Receive_Ok_Response_From_Get_Ping_Request()
        {
            // Arrange.
            var controller = new SpyderByteAPI.Controllers.Health.V1_4.HealthController();

            // Act.
            var actual = await controller.Ping();

            // Assert.
            actual.Should().BeOfType<OkResult>();
        }
    }
}
