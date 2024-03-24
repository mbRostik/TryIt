using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Contracts.DTOs;
using Users.Application.Contracts.Interfaces;
using Users.Application.UseCases.Queries;
using Users.Infrastructure.Data;

namespace Users.Application.UseCases.Handlers.QueryHandlers
{
    public class GetUserHandler : IRequestHandler<GetUserQuery, UserProfileDTO>
    {

        private readonly UserDbContext dbContext;
        private readonly IMapper mapper;
        public readonly Serilog.ILogger logger;

        public GetUserHandler(UserDbContext dbContext, IMapperService mapperService, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            mapperService.Mapper_UserToUserProfileDTO(ref mapper);
            this.logger = logger;
        }

        public async Task<UserProfileDTO> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                logger.Information("Starting to handle GetUserQuery for user ID {UserId}", request.id);

                var dbUser = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == request.id);

                int FollowerCount = await dbContext.Follows.AsNoTracking().CountAsync(u => u.UserId == request.id);
                int FollowsCount = await dbContext.Follows.AsNoTracking().CountAsync(u => u.FollowerId == request.id);
                UserProfileDTO userInfo = mapper.Map<UserProfileDTO>(dbUser);

                if(FollowerCount!=null && FollowsCount != null)
                {
                    userInfo.FollowersCount = FollowerCount;
                    userInfo.FollowsCount = FollowsCount;
                }
                logger.Information("Successfully handled GetUserQuery for user ID {UserId}.}", request.id);

                return userInfo;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while handling GetUserQuery for user ID {UserId}", request.id);
                return null;
            }
            
        }
    }
}