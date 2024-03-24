using Chats.Application.UseCases.Commands;
using Chats.Domain.Entities;
using Chats.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.UseCases.Handlers.OperationHandlers
{
    public class UserCreatedHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly IMediator mediator;

        private readonly ChatDbContext dbContext;
        private readonly Serilog.ILogger logger;

        public UserCreatedHandler(ChatDbContext dbContext, IMediator mediator, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var model = await dbContext.Users.AddAsync(request.model);
                await dbContext.SaveChangesAsync();

                logger.Information($"User {model.Entity.Id} created successfully");
                return model.Entity;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error creating user");
                return null;
            }
        }
    }
}
