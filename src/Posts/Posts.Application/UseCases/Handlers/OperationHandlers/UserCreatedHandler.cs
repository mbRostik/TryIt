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

        public UserCreatedHandler(PostDbContext dbContext, IMediator mediator)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
        }

        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var model = await dbContext.Users.AddAsync(request.model);

                await dbContext.SaveChangesAsync();

                return model.Entity;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
