using Chats.Application.UseCases.Queries;
using Chats.Domain.Entities;
using Chats.Infrastructure.Data;
using MediatR;
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

        public GetAllChatsHandler(ChatDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Chat>> Handle(GetAllChatsQuery request, CancellationToken cancellationToken)
        {
            var result = dbContext.Chats.ToList();
            return result;
        }


    }
}

