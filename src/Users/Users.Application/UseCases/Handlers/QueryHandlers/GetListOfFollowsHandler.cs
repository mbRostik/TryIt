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
    public class GetListOfFollowsHandler : IRequestHandler<GetListOfFollowsQuery, List<Follow>>
    {
        public readonly Serilog.ILogger logger;

        private readonly UserDbContext dbContext;
        public GetListOfFollowsHandler(UserDbContext dbContext, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<List<Follow>> Handle(GetListOfFollowsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                logger.Information("Handling GetListOfFollowsQuery for {Count} ids.", request.userId);

                var result = dbContext.Follows
                    .AsNoTracking()
                    .Where(x => x.FollowerId == request.userId).ToList();
                
                logger.Information("Successfully handled GetListOfFollowsQuery.");
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while handling GetListOfFollowsQuery.");
                throw;
            }
        }
    }
}