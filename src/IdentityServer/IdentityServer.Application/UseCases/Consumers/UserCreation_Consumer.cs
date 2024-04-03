using MassTransit;
using MessageBus.Messages.Events.IdentityServerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Application.UseCases.Consumers
{
    public class UserCreation_Consumer : IConsumer<IUserCreate_Processed>
    {
        public async Task Consume(ConsumeContext<IUserCreate_Processed> context)
        {
            Console.WriteLine($"User processed to {context.Message.CorrelationId} was received");
        }
    }
}
