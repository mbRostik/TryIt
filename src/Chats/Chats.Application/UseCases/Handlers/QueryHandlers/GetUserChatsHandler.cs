using Chats.Application.Contracts.DTOs;
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
    public class GetUserChatsHandler : IRequestHandler<GetUserChatsQuery, IEnumerable<GiveUserChatsDTO>>
    {

        private readonly ChatDbContext dbContext;

        public GetUserChatsHandler(ChatDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<GiveUserChatsDTO>> Handle(GetUserChatsQuery request, CancellationToken cancellationToken)
        {
            return null;
        }


    }
}
