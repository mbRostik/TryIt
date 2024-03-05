using MediatR;
using Reports.Application.UseCases.Commands;
using Reports.Domain;
using Reports.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Application.UseCases.Handlers.OperationHandlers
{
    public class UserCreatedHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly IMediator mediator;

        private readonly ReportDbContext dbContext;

        public UserCreatedHandler(ReportDbContext dbContext, IMediator mediator)
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