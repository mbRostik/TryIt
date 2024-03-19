using Chats.Application.Contracts.DTOs;
using Chats.Application.UseCases.Commands;
using Chats.Domain.Entities;
using Chats.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chats.Application.UseCases.Handlers.OperationHandlers
{
    public class MessageCreatedHandler : IRequestHandler<CreateMessageCommand, int>
    {
        private readonly IMediator mediator;

        private readonly ChatDbContext dbContext;

        public MessageCreatedHandler(ChatDbContext dbContext, IMediator mediator)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
        }

        public async Task<int> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.message.ChatId == null)
                {
                    var ChatId = await mediator.Send(new CreateChatCommand(request.SenderId, request.message.ReceiverId));
                    //Mapper потім добавлю XD

                    Message message = new Message
                    {
                        ChatId = ChatId,
                        SenderId = request.SenderId,
                        Content = request.message.MessageContent
                    };

                    var result = dbContext.Messages.AddAsync(message);
                    await dbContext.SaveChangesAsync();
                    if(!result.IsCompletedSuccessfully) { return 0; }


                    return ChatId;
                }
                else
                {
                    Message message = new Message
                    {
                        ChatId = request.message.ChatId ?? 0,
                        SenderId = request.SenderId,
                        Content = request.message.MessageContent
                    };

                    var result = dbContext.Messages.AddAsync(message);
                    await dbContext.SaveChangesAsync();

                    return request.message.ChatId??0;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return 0;
            }
        }
    }
}
