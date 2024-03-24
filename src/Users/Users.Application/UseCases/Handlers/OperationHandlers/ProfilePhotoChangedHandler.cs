using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Contracts.Interfaces;
using Users.Application.UseCases.Commands;
using Users.Infrastructure.Data;
using Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Users.Application.Contracts.DTOs;
using Users.Application.UseCases.Queries;

namespace Users.Application.UseCases.Handlers.OperationHandlers
{
    public class ProfilePhotoChangedHandler : IRequestHandler<ChangeUserAvatarCommand, UserProfileDTO>
    {
        private readonly IMediator mediator;

        private readonly UserDbContext dbContext;
        private readonly IMapper mapper;
        public readonly Serilog.ILogger logger;

        public ProfilePhotoChangedHandler(UserDbContext dbContext, IMediator mediator, IMapperService mapperService, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            mapperService.Mapper_ChangeUserProfileToUserDTO(ref mapper);
            this.logger = logger;
        }

        public async Task<UserProfileDTO> Handle(ChangeUserAvatarCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.Information("Attempting to change avatar for user ID {UserId}", request.model.Id);

                var user = await dbContext.Users.FindAsync(new object[] { request.model.Id }, cancellationToken);
                if (user != null)
                {
                    user.Photo = Convert.FromBase64String(request.model.Avatar);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    logger.Information("Avatar changed successfully for user ID {UserId}. Fetching updated user profile.", request.model.Id);

                    var result = await mediator.Send(new GetUserQuery(request.model.Id), cancellationToken);

                    if (result == null)
                    {
                        logger.Warning("Updated user profile for user ID {UserId} not found after changing avatar.", request.model.Id);
                        return null; 
                    }

                    return result;
                }
                else
                {
                    logger.Warning("User with ID {UserId} not found. Cannot change avatar.", request.model.Id);
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while changing avatar for user ID {UserId}", request.model.Id);
                throw; 
            }
        }
    }
}