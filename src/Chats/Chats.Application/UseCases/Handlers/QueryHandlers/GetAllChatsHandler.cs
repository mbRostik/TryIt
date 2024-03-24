using Chats.Application.UseCases.Queries;
using Chats.Domain.Entities;
using Chats.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.UseCases.Handlers.QueryHandlers
{

    public class GetAllChatsHandler : IRequestHandler<GetAllChatsQuery, IEnumerable<Chat>>
    {

        private readonly ChatDbContext dbContext;
        private readonly Serilog.ILogger logger;

        public GetAllChatsHandler(ChatDbContext dbContext, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<IEnumerable<Chat>> Handle(GetAllChatsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                logger.Information("Fetching all chats");
                var result = dbContext.Chats.ToList();

                if (result.Any())
                {
                    logger.Information($"Retrieved {result.Count} chats successfully");
                }
                else
                {
                    logger.Information("No chats found");
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error retrieving all chats");
                throw; 
            }
        }


    }
}

