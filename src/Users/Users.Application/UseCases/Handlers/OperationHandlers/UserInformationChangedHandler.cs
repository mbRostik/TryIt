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
using Users.Application.UseCases.Commands;
using Users.Domain.Entities;
using Users.Infrastructure.Data;

namespace Users.Application.UseCases.Handlers.OperationHandlers
{
    public class UserInformationChangedHandler : IRequestHandler<ChangeUserInformationCommand>
    {
        private readonly IMediator mediator;

        private readonly UserDbContext dbContext;
        private readonly IMapper mapper;

        public UserInformationChangedHandler(UserDbContext dbContext, IMediator mediator, IMapperService mapperService)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            mapperService.Mapper_ChangeUserProfileToUserDTO(ref mapper);
        }

        public async Task Handle(ChangeUserInformationCommand request, CancellationToken cancellationToken)
        {
            User userInfo = mapper.Map<User>(request.model);
            userInfo.SexId = 2;
            dbContext.Users.Update(userInfo);

            await dbContext.SaveChangesAsync();
        }
    }
}