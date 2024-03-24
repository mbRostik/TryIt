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
    public class GetListUsersHandler : IRequestHandler<GetListUsersQuery, List<UserChatProfileDTO>>
    {
        public readonly Serilog.ILogger logger;

        private readonly UserDbContext dbContext;
        private readonly IMapper mapper;
        public GetListUsersHandler(UserDbContext dbContext, IMapperService mapperService, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            mapperService.Mapper_UserToUserChatProfileDTO(ref mapper);
            this.logger = logger;
        }

        public async Task<List<UserChatProfileDTO>> Handle(GetListUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                logger.Information("Handling GetListUsersQuery for {Count} ids.", request.ids.Count);

                var result = new List<UserChatProfileDTO>();

                foreach (var id in request.ids)
                {
                    var dbUser = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
                    if (dbUser != null)
                    {
                        var userInfo = mapper.Map<UserChatProfileDTO>(dbUser);
                        result.Add(userInfo);
                        logger.Information("Mapped user {UserId} to UserChatProfileDTO.", id);
                    }
                    else
                    {
                        logger.Warning("User with id {UserId} not found.", id);
                    }
                }

                logger.Information("Successfully handled GetListUsersQuery. Total users mapped: {MappedUsersCount}.", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while handling GetListUsersQuery.");
                throw; 
            }
        }
    }
}