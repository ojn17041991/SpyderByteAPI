using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using SpyderByteServices.Services.Imgur;
using SpyderByteServices.Services.Imgur.Abstract;
using System.Net;

namespace SpyderByteTest.Services.ImgurServiceTests.Helpers
{
    public class ImgurServiceHelper
    {
        private const string TOKEN_ENDPOINT = "oauth2/token";
        private const string IMAGE_ENDPOINT = "image";
        private const string ERROR = "error";

        private readonly Fixture _fixture;

        private IConfiguration _configuration;
        private readonly Mock<IHttpClientFactory> _httpClientFactory;

        public IImgurService Service;

        public ImgurServiceHelper()
        {
            var formFile = new Mock<IFormFile>();
            formFile.Setup(f => f.ContentType).Returns("image/png");
            formFile.Setup(f => f.FileName).Returns("test-file");

            _fixture = new Fixture();
            _fixture.Customize<IFormFile>(f => f.FromFactory(() => formFile.Object));

            _configuration = new Mock<IConfiguration>().Object;

            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()
            ).ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
            {
                // Allows us to control the failures. Don't ever want the token request to fail in this way though.
                if (request.RequestUri!.AbsoluteUri.Contains(ERROR) && !request.RequestUri!.AbsoluteUri.Contains(TOKEN_ENDPOINT))
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Content = new StringContent(string.Empty)
                    };
                }

                if (request.Method == HttpMethod.Post)
                {
                    if (request.RequestUri!.AbsoluteUri.Contains(TOKEN_ENDPOINT))
                    {
                        return new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(@$"{{""access_token"": ""{Guid.NewGuid()}""}}")
                        };
                    }
                    else if (request.RequestUri!.AbsoluteUri.Contains(IMAGE_ENDPOINT))
                    {
                        return new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(@$"{{""data"":{{""id"":""{_fixture.Create<int>()}"",""link"":""https://i.imgur.com/{_fixture.Create<int>()}.png""}}}}")
                        };
                    }
                }
                else if (request.Method == HttpMethod.Delete)
                {
                    if (request.RequestUri!.AbsoluteUri.Contains(IMAGE_ENDPOINT))
                    {
                        return new HttpResponseMessage
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = new StringContent(string.Empty)
                        };
                    }
                }

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(string.Empty)
                };
            });

            _httpClientFactory = new Mock<IHttpClientFactory>();
            _httpClientFactory.Setup(h =>
                h.CreateClient(It.IsAny<string>())
            ).Returns((string name) =>
                new HttpClient(httpMessageHandler.Object)
            );

            Service = new ImgurService(_configuration!, _httpClientFactory.Object);
        }

        public IFormFile GenerateFormFile()
        {
            return _fixture.Create<IFormFile>();
        }

        public void BuildFullConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string?>>
                {
                    new KeyValuePair<string, string?>("Imgur:Url", "http://127.0.0.1/"),
                    new KeyValuePair<string, string?>("Imgur:RefreshToken", Guid.NewGuid().ToString())
                })
                .Build();
            Service = new ImgurService(_configuration!, _httpClientFactory.Object);
        }

        public void BuildHttpRequestErrorConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string?>>
                {
                    new KeyValuePair<string, string?>("Imgur:Url", $"http://127.0.0.1/{ERROR}/"),
                    new KeyValuePair<string, string?>("Imgur:RefreshToken", Guid.NewGuid().ToString())
                })
                .Build();
            Service = new ImgurService(_configuration!, _httpClientFactory.Object);
        }

        public void BuildUnathorizedConfiguration()
        {
            _configuration = new Mock<IConfiguration>().Object;
            Service = new ImgurService(_configuration!, _httpClientFactory.Object);
        }
    }
}