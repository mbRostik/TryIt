using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.UseCases.Commands;
using Users.Domain.Entities;
using Users.Infrastructure.Data;

namespace Users.Application.UseCases.Handlers.OperationHandlers
{
    public class UserCreatedHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly IMediator mediator;
        public readonly Serilog.ILogger logger;

        private readonly UserDbContext dbContext;

        public UserCreatedHandler(UserDbContext dbContext, IMediator mediator, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.Information("Creating new user with details: {UserDetails}", request.model);

                var model = await dbContext.Users.AddAsync(request.model, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                logger.Information("Successfully created user with ID: {UserId}", model.Entity.Id);

                return model.Entity;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error occurred while creating user. Details: {UserDetails}", request.model);
                throw; 
            }
        }
    }
}
