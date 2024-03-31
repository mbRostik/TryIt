using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Contracts.DTOs;
using Users.Application.UseCases.Commands;
using Users.Application.UseCases.Queries;
using Users.Domain.Entities;
using Users.Infrastructure.Data;

namespace Users.Application.UseCases.Handlers.OperationHandlers
{
    public class FollowingCreatedHandler : IRequestHandler<CreateFollowingCommand, UserProfileDTO>
    {
        private readonly IMediator mediator;

        private readonly UserDbContext dbContext;
        public readonly Serilog.ILogger logger;

        public FollowingCreatedHandler(UserDbContext dbContext, IMediator mediator, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task<UserProfileDTO> Handle(CreateFollowingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.Information("Attempting to start following {UserId}", request.profileId);

                var entityToRemove = await dbContext.Follows
                  .FirstOrDefaultAsync(x => x.FollowerId == request.followerId && x.UserId == request.profileId);

                if (entityToRemove != null)
                {
                    dbContext.Follows.Remove(entityToRemove);
                    await dbContext.SaveChangesAsync();
                    var unf_result = await mediator.Send(new GetUserProfileQuery(request.profileId));
                    return unf_result;
                }
                Follow temp = new Follow
                {
                    FollowerId = request.followerId,
                    UserId = request.profileId
                };

                await dbContext.Follows.AddAsync(temp);
                await dbContext.SaveChangesAsync();

                var result = await mediator.Send(new GetUserProfileQuery(request.profileId));

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while follow {UserId}", request.profileId);
                throw;
            }
        }
    }
}