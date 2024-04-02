using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.Contracts.Interfaces;
using Users.Application.UseCases.Queries;
using Users.Domain.Entities;
using Users.Infrastructure.Data;

namespace Users.Application.UseCases.Handlers.QueryHandlers
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, User>
    {

        private readonly UserDbContext dbContext;
        public readonly Serilog.ILogger logger;

        public GetUserByIdHandler(UserDbContext dbContext,  Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                logger.Information("Starting to handle GetUserByIdQuery for user ID {UserId}", request.id);

                var dbUser = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == request.id);

                logger.Information("Successfully handled GetUserByIdQuery for user ID {UserId}.}", request.id);

                return dbUser;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while handling GetUserByIdQuery for user ID {UserId}", request.id);
                return null;
            }

        }
    }
}