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
using Users.Domain.Enums;
using Users.Infrastructure.Data;

namespace Users.Application.UseCases.Handlers.OperationHandlers
{
    public class UserInformationChangedHandler : IRequestHandler<ChangeUserInformationCommand>
    {
        private readonly IMediator mediator;

        private readonly UserDbContext dbContext;
        private readonly IMapperService _mapper;
        public readonly Serilog.ILogger _logger;


        public UserInformationChangedHandler(UserDbContext dbContext, IMediator mediator, IMapperService mapperService, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            _mapper = mapperService;
            _logger = logger;
        }

        public async Task Handle(ChangeUserInformationCommand request, CancellationToken cancellationToken)
        {

            _logger.Information("Attempting to change user information for UserId: {UserId}", request.model.Id);

            try
            {
                var mapper = _mapper.Mapper_ChangeUserProfileToUserDTO();
                User userInfo = mapper.Map<User>(request.model);

                dbContext.Users.Update(userInfo);
                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.Information("User information successfully changed for UserId: {UserId}", userInfo.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while changing user information for UserId: {UserId}", request.model.Id);

                throw;
            }
        }
    }
}