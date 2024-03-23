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
        public GetUserHandler(UserDbContext dbContext, IMapperService mapperService)
        {
            this.dbContext = dbContext;
            mapperService.Mapper_UserToUserProfileDTO(ref mapper);
        }

        public async Task<UserProfileDTO> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var dbUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.id);

                int FollowerCount = await dbContext.Follows.CountAsync(u => u.UserId == request.id);
                int FollowsCount = await dbContext.Follows.CountAsync(u => u.FollowerId == request.id);
                UserProfileDTO userInfo = mapper.Map<UserProfileDTO>(dbUser);

                if(FollowerCount!=null && FollowsCount != null)
                {
                    userInfo.FollowersCount = FollowerCount;
                    userInfo.FollowsCount = FollowsCount;
                }
                
                return userInfo;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }
    }
}