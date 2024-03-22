﻿using AutoMapper;
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

        public ProfilePhotoChangedHandler(UserDbContext dbContext, IMediator mediator, IMapperService mapperService)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            mapperService.Mapper_ChangeUserProfileToUserDTO(ref mapper);
        }

        public async Task<UserProfileDTO> Handle(ChangeUserAvatarCommand request, CancellationToken cancellationToken)
        {

            var user = await dbContext.Users.FindAsync(request.model.Id);
            if (user != null)
            {
                user.Photo = Convert.FromBase64String(request.model.Avatar);
                await dbContext.SaveChangesAsync();

                var result = await mediator.Send(new GetUserQuery(request.model.Id));

                return result;
            }
            return null;
        }
    }
}