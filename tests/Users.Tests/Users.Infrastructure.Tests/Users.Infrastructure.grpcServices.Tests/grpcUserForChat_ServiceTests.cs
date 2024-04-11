using AutoMapper;
using Google.Protobuf;
using Grpc.Core;
using MediatR;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Userforchat;
using Users.Application.Contracts.DTOs;
using Users.Application.Contracts.Interfaces;
using Users.Application.UseCases.Queries;
using Users.Infrastructure.Services.grpcServices;
using Xunit;
namespace Users.Infrastructure.grpcServices.Tests
{
    public class grpcUserForChat_ServiceTests
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<IMapperService> _mapperMock = new Mock<IMapperService>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly grpcUserForChat_Service _service;

        public grpcUserForChat_ServiceTests()
        {
            _service = new grpcUserForChat_Service(_mediatorMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetUserForChat_ShouldReturnUserForChat_WhenUsersExist()
        {
            // Arrange
            var request = new GetUserForChatRequest { UserId = { "1", "2" } };
            var users = new List<UserChatProfileDTO>
                    {
                        new UserChatProfileDTO {  NickName = "User1", Photo = new byte[] { 1, 2, 3 } },
                        new UserChatProfileDTO {  NickName = "User2", Photo = new byte[] { 4, 5, 6 } }
                    };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetListUsersQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(users);

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserChatProfileDTO, GiveUserForChat>()
                    .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => Google.Protobuf.ByteString.CopyFrom(src.Photo)));
            });
            var mapper = mapperConfiguration.CreateMapper();
            _mapperMock.Setup(m => m.Mapper_UserChatProfileToGiveUserForChat()).Returns(mapper);

            // Act
            var response = await _service.GetUserForChat(request, It.IsAny<ServerCallContext>());

            // Assert
            Assert.NotNull(response);
            Assert.Equal(users.Count, response.Users.Count);
            foreach (var user in response.Users)
            {
                var expectedUser = users.FirstOrDefault(u => u.NickName == user.NickName);
                Assert.NotNull(expectedUser);
                Assert.Equal(expectedUser.NickName, user.NickName);
                Assert.True(expectedUser.Photo.SequenceEqual(user.Photo.ToByteArray()), "Photo byte array mismatch");
            }

            _loggerMock.Verify(log => log.Information(It.IsAny<string>()), Times.Exactly(2));
        }


        [Fact]
        public async Task GetUserForChat_ShouldReturnEmptyUser_WhenUsersDoNotExist()
        {
            // Arrange
            var request = new GetUserForChatRequest { UserId = { "3" } };
            List<UserProfileDTO> users = null;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetListUsersQuery>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((List<UserChatProfileDTO>)null);



            // Act
            var response = await _service.GetUserForChat(request, It.IsAny<ServerCallContext>());

            // Assert
            Assert.NotNull(response);
            Assert.Single(response.Users);
            Assert.Equal("0", response.Users[0].UserId);
            _loggerMock.Verify(log => log.Warning(It.IsAny<string>()), Times.Once);
        }
    }
}