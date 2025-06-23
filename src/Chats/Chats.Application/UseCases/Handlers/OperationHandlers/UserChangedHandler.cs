using Chats.Application.UseCases.Commands;
using Chats.Domain.Entities;
using Chats.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.UseCases.Handlers.OperationHandlers
{
    public class UserChangedHandler : IRequestHandler<ChangeUserCommand, User>
    {
        private readonly ChatDbContext dbContext;
        private readonly Serilog.ILogger logger;

        public UserChangedHandler(ChatDbContext dbContext, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<User> Handle(ChangeUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var model = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.model.Id);

                if (model == null)
                {
                    throw new KeyNotFoundException();
                }

                model.IsCheckingMessages = request.model.IsCheckingMessages;
                await dbContext.SaveChangesAsync();

                logger.Information($"User {model.Id} changed successfully");
                return model;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error changing user");
                return null;
            }
        }
    }
}