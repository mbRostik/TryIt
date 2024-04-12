using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Moq;
using Posts.Application.Contracts.DTOs;
using Posts.Application.UseCases.Queries;
using Posts.Infrastructure.Services.grpcServices;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Userposts;

namespace Posts.Infrastructure.grpcServices.Tests
{
    public class grpcUserPosts_ServiceTests
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly grpcUserPosts_Service _service;

        public grpcUserPosts_ServiceTests()
        {
            _service = new grpcUserPosts_Service(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetUserPosts_ReturnsPosts_WhenPostsExist()
        {
            // Arrange
            var request = new GetUserPostsRequest { UserId = "1" };
            var posts = new List<GiveUserPosts>
        {
            new GiveUserPosts { Id = 1, Title = "Post1", Content = "Content1", Date = Timestamp.FromDateTime(DateTime.UtcNow)}
        };
            PFile tempFile = new PFile
            {
                Name = "efef",
                PostId = 1,
                File = Google.Protobuf.ByteString.CopyFrom(new byte[1])
            };

            posts[0].Files.Add(tempFile);


            _mediatorMock.Setup(m => m.Send(It.IsAny<GiveProfilePostsDTO>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(posts);

            // Act
            var response = await _service.GetUserPosts(request, It.IsAny<ServerCallContext>());

            // Assert
            Assert.NotNull(response);
            Assert.Equal(posts.Count, response.Posts.Count);
            _loggerMock.Verify(log => log.Information(It.IsAny<string>()), Times.AtLeastOnce());
        }

        [Fact]
        public async Task GetUserPosts_ReturnsEmpty_WhenNoPostsExist()
        {
            // Arrange
            var request = new GetUserPostsRequest { UserId = "2" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GiveProfilePostsDTO>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync((List<GiveUserPosts>)null);

            // Act
            var response = await _service.GetUserPosts(request, It.IsAny<ServerCallContext>());

            // Assert
            Assert.NotNull(response);
            _loggerMock.Verify(log => log.Warning(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetUserPosts_CatchesExceptions()
        {
           //Arrange
           var request = new GetUserPostsRequest { UserId = "3" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserPostsQuery>(), It.IsAny<CancellationToken>()))
                          .ThrowsAsync(new Exception("Test exception"));

           //Act
           var response = await _service.GetUserPosts(request, It.IsAny<ServerCallContext>());

            //Assert
            Assert.NotNull(response);
            _loggerMock.Verify(log => log.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }
    }

  
}
