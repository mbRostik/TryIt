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

        private readonly UserDbContext dbContext;
        private readonly IMapper mapper;
        public GetListUsersHandler(UserDbContext dbContext, IMapperService mapperService)
        {
            this.dbContext = dbContext;
            mapperService.Mapper_UserToUserChatProfileDTO(ref mapper);
        }

        public async Task<List<UserChatProfileDTO>> Handle(GetListUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new List<UserChatProfileDTO>();

                foreach (var id in request.ids)
                {
                    var dbUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
                    if (dbUser != null)
                    {
                        var userInfo = mapper.Map<UserChatProfileDTO>(dbUser);
                        result.Add(userInfo);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }
    }
}