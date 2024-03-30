using MediatR;
using Posts.Application.UseCases.Commands;
using Posts.Domain.Entities;
using Posts.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.UseCases.Handlers.OperationHandlers
{
    public class UserCreatedHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly IMediator mediator;

        private readonly PostDbContext dbContext;
        private readonly Serilog.ILogger logger;

        public UserCreatedHandler(PostDbContext dbContext, IMediator mediator, Serilog.ILogger logger)
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

                logger.Information("User with ID {UserId} created successfully", model.Entity.Id);

                return model.Entity;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error creating user. {ErrorMessage}", ex.Message);
                return null;
            }
        }
    }
}
