using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Contracts.DTOs;
using Users.Application.UseCases.Queries;
using Users.Infrastructure.Data;

namespace Users.Application.UseCases.Handlers.QueryHandlers
{
    public class GetUserFriendsHandler : IRequestHandler<GetUserFriendsQuery, List<GiveSmbProfileDTO>>
    {
        public readonly Serilog.ILogger _logger;

        private readonly UserDbContext dbContext;
        public GetUserFriendsHandler(UserDbContext dbContext, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<List<GiveSmbProfileDTO>> Handle(GetUserFriendsQuery request, CancellationToken cancellationToken)
        {
            _logger.Information("Processing GetUserFriendsQuery for user ID {UserId}", request.userId);
            try
            {
                var followingIds = await dbContext.Follows
                    .Where(f => f.FollowerId == request.userId)
                    .Select(f => f.UserId)
                    .ToListAsync();

                var followerIds = await dbContext.Follows
                    .Where(f => followingIds.Contains(f.FollowerId) && f.UserId == request.userId)
                    .Select(f => f.FollowerId.ToString())
                    .ToListAsync();

                var mutualFollowersIds = followingIds.Intersect(followerIds).ToList();

                var friends = await dbContext.Users
                    .Where(u => mutualFollowersIds.Contains(u.Id))
                    .ToListAsync();


                if (!friends.Any())
                {
                   _logger.Information("No friends found for user ID {UserId}", request.userId);

                    return null;
                }

                List<GiveSmbProfileDTO> result = new List<GiveSmbProfileDTO>();

                foreach (var user in friends)
                {
                    GiveSmbProfileDTO temp = new GiveSmbProfileDTO
                    {
                        Id = user.Id,
                        Email = user.Email,
                        NickName = user.NickName,
                        Name = user.Name,
                        Phone = user.Phone,
                        Bio = user.Bio,
                        Photo = user.Photo,
                        DateOfBirth = user.DateOfBirth,
                        IsPrivate = user.IsPrivate,
                        isFollowedByUser = true
                };
               
                    result.Add(temp);
                }
                _logger.Information("GetUserFriendsQuery for user ID {UserId} successfully processed. Friends found: {FriendsCount}", request.userId, result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error processing GetUserFriendsQuery for user ID {UserId}", request.userId);

                return null;
            }
        }
    }
}