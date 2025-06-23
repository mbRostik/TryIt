using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Posts.Application.Contracts.DTOs;
using Posts.Application.UseCases.Queries;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Posts.IntegrationTests
{
    public class PostControllerIntegrationTests : IClassFixture<CustomWebApplicationFactoryForPost>
    {
        private HttpClient _client;
        private Mock<IMediator> _mockMediator;
        private Mock<ILogger> _mockLogger;

        public PostControllerIntegrationTests(CustomWebApplicationFactoryForPost factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    _mockMediator = new Mock<IMediator>();
                    _mockLogger = new Mock<ILogger>();

                    services.AddTransient(_ => _mockMediator.Object);
                    services.AddTransient(_ => _mockLogger.Object);
                });
            }).CreateClient();
        }
        [Fact]
        public async Task GetUserPosts_ReturnsOk_WithPosts()
        {
            var response = await _client.GetAsync("/Post/GetUserPosts");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var posts = JsonConvert.DeserializeObject<List<GiveProfilePostsDTO>>(responseString);

        }


        [Fact]
        public async Task CreatePost_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            var invalidModel = new CreatePostDTO { Title = "e", Content = "ef", UserId = "1" };

            // Act
            var response = await _client.PostAsJsonAsync("/CreatePost", invalidModel);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            _mockLogger.Verify(l => l.Error(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }
    }
}