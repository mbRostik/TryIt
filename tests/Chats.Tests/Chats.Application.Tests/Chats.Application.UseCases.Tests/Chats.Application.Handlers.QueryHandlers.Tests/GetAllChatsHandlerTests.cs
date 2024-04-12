using Chats.Application.UseCases.Handlers.QueryHandlers;
using Chats.Application.UseCases.Queries;
using Chats.Domain.Entities;
using Chats.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.UseCases.Tests.Chats.Application.Handlers.QueryHandlers.Tests
{
    public class GetAllChatsHandlerTests
    {
        private readonly ChatDbContext _dbContext;
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();
        private readonly GetAllChatsHandler _handler;

        public GetAllChatsHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ChatDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForGetAllChatsHandler")
                .Options;

            _dbContext = new ChatDbContext(options);
            _handler = new GetAllChatsHandler(_dbContext, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnAllChats_WhenChatsExist()
        {
            // Arrange
            var chats = new[]
            {
                new Chat { Id = 1 },
                new Chat { Id = 2 }
            };

            _dbContext.Chats.AddRange(chats);
            await _dbContext.SaveChangesAsync();

            var request = new GetAllChatsQuery();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(chats.Length);
            result.Select(r => r.Id).Should().BeEquivalentTo(chats.Select(c => c.Id));

            _loggerMock.Verify(log => log.Information(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_ShouldLogAppropriateMessages_WhenNoChatsExist()
        {
            // Ensure the database is clean
            _dbContext.Chats.RemoveRange(_dbContext.Chats);
            await _dbContext.SaveChangesAsync();

            var request = new GetAllChatsQuery();

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();

            _loggerMock.Verify(log => log.Information("Fetching all chats"), Times.Once);
            _loggerMock.Verify(log => log.Information("No chats found"), Times.Once);
        }
    }
}
