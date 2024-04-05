using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Contracts.DTOs;
using Users.Application.UseCases.Queries;
using Users.Domain.Entities;
using Users.Infrastructure.Data;

namespace Users.Application.UseCases.Handlers.QueryHandlers
{
    public class GetUsersByLettersHandler : IRequestHandler<GetUsersByLettersQuery, List<GiveSmbProfileDTO>>
    {
        public readonly Serilog.ILogger logger;

        private readonly UserDbContext dbContext;
        public GetUsersByLettersHandler(UserDbContext dbContext, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<List<GiveSmbProfileDTO>> Handle(GetUsersByLettersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var usersQuery = await dbContext.Users
                             .Where(u => u.NickName.Contains(request.SearchingField))
                             .OrderBy(u => u.NickName.StartsWith(request.SearchingField) ? 0 : 1)
                             .ThenBy(u => u.NickName).ToListAsync();
                if (!usersQuery.Any())
                {
                    return null;
                }

                List<GiveSmbProfileDTO> result = new List<GiveSmbProfileDTO>();

                foreach (var user in usersQuery)
                {
                    int followerCount = await dbContext.Follows.AsNoTracking().CountAsync(u => u.UserId == user.Id);
                    int followsCount = await dbContext.Follows.AsNoTracking().CountAsync(u => u.FollowerId == user.Id);

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
                        FollowersCount = followerCount,
                        FollowsCount = followsCount
                    };
                    var isFollowed = await dbContext.Follows
                        .FirstOrDefaultAsync(x => x.FollowerId == request.userId && x.UserId == user.Id);

                    if (isFollowed != null)
                    {
                        temp.isFollowedByUser = true;
                    }
                    result.Add(temp);
                }
               

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}