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
    public class GetSmbProfileHandler : IRequestHandler<GetSmbProfileQuery, GiveSmbProfileDTO>
    {

        private readonly UserDbContext dbContext;
        private readonly IMapperService _mapper;
        public readonly Serilog.ILogger logger;
        private readonly IMediator mediator;

        public GetSmbProfileHandler(UserDbContext dbContext, IMapperService mapperService, Serilog.ILogger logger, IMediator mediator)
        {
            this.dbContext = dbContext;
            _mapper = mapperService;
            this.logger = logger;
            this.mediator = mediator;
        }

        public async Task<GiveSmbProfileDTO> Handle(GetSmbProfileQuery request, CancellationToken cancellationToken)
        {
            var mapper = _mapper.Mapper_UserToUserProfileDTO();
            
            try
            {
                logger.Information("Starting to handle GetSmbProfileQuery for user ID {UserId}", request.ProfileId);

                var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == request.ProfileId);

                int followerCount = await dbContext.Follows.AsNoTracking().CountAsync(u => u.UserId == request.ProfileId);
                int followsCount = await dbContext.Follows.AsNoTracking().CountAsync(u => u.FollowerId == request.ProfileId);

                GiveSmbProfileDTO userInfo = new GiveSmbProfileDTO
                {
                    Email = user.Email,
                    NickName = user.NickName,
                    Name = user.Name,
                    Phone = user.Phone,
                    Bio = user.Bio,
                    Photo = user.Photo,
                    DateOfBirth = user.DateOfBirth,
                    IsPrivate = user.IsPrivate,
                    FollowersCount = followerCount,
                    FollowsCount = followsCount
                };


                var isFollowed = await dbContext.Follows
                  .FirstOrDefaultAsync(x => x.FollowerId == request.UserId && x.UserId == request.ProfileId);

                if (isFollowed != null)
                {
                    userInfo.isFollowedByUser = true;
                    return userInfo;
                }
                logger.Information("Successfully handled GetSmbProfileQuery for user ID {UserId}.}", request.ProfileId);

                return userInfo;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while handling GetSmbProfileQuery for user ID {UserId}", request.ProfileId);
                return null;
            }

        }
    }
}