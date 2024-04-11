using AutoMapper;
using Google.Protobuf;
using Grpc.Core;
using MediatR;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Userforpost;
using Users.Application.UseCases.Queries;
using Users.Domain.Entities;
using Xunit;

namespace Users.Infrastructure.Services.grpcServices.Tests
{
    public class grpcUserForPost_ServiceTests
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly grpcUserForPost_Service _service;

        public grpcUserForPost_ServiceTests()
        {
            _service = new grpcUserForPost_Service(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetUserForPost_ShouldReturnUserForPost_WhenFollowersExist()
        {
            // Arrange
            var request = new GetUserForPostRequest { UserId = "1" };
            var followers = new List<Follow>
            {
                new Follow { UserId = "2", FollowerId="3" },
                new Follow { UserId = "3", FollowerId="2" }
            };
            var user1 = new User { Id = "2",
                Email = "user@example.com",
                NickName = "UserNick2",
                Name = "UserName2",
                Phone = "1234567892",
                Bio = "User Bio",
                Photo = new byte[0],
                DateOfBirth = DateTime.Now.AddYears(-20),
                IsPrivate = false
            };
            var user2 = new User { Id = "3",
                Email = "user@example.com",
                NickName = "UserNick21",
                Name = "UserName22",
                Phone = "12345678922",
                Bio = "User Bio",
                Photo = new byte[0],
                DateOfBirth = DateTime.Now.AddYears(-20),
                IsPrivate = false
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetListOfFollowsQuery>(), default)).ReturnsAsync(followers);
            _mediatorMock.SetupSequence(m => m.Send(It.IsAny<GetUserByIdQuery>(), default))
                         .ReturnsAsync(user1)
                         .ReturnsAsync(user2);

            // Act
            var response = await _service.GetUserForPost(request, It.IsAny<ServerCallContext>());

            // Assert
            Assert.NotNull(response);
            Assert.Equal(followers.Count, response.Users.Count);
            Assert.Collection(response.Users,
                user =>
                {
                    Assert.Equal(user1.Id, user.UserId);
                    Assert.Equal(user1.NickName, user.NickName);
                    Assert.True(user1.Photo.SequenceEqual(user.Photo.ToByteArray()));
                },
                user =>
                {
                    Assert.Equal(user2.Id, user.UserId);
                    Assert.Equal(user2.NickName, user.NickName);
                    Assert.True(user2.Photo.SequenceEqual(user.Photo.ToByteArray()));
                });

            _loggerMock.Verify(log => log.Information(It.IsAny<string>()), Times.Exactly(3));
        }

        [Fact]
        public async Task GetUserForPost_ShouldReturnEmptyUser_WhenFollowersDoNotExist()
        {
            // Arrange
            var request = new GetUserForPostRequest { UserId = "1" };
            List<Follow> followers = null;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetListOfFollowsQuery>(), default)).ReturnsAsync(followers);

            // Act
            var response = await _service.GetUserForPost(request, It.IsAny<ServerCallContext>());

            // Assert
            Assert.NotNull(response);
            Assert.Single(response.Users);
            Assert.Equal("0", response.Users[0].UserId);
            _loggerMock.Verify(log => log.Warning(It.IsAny<string>()), Times.Once);
        }
    }
}
