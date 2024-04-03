using MessageBus.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Messages.Commands.IdentityServerService
{
    public interface IUserCreate_Send_To_SubscriptionWebApi : IMessage<UserCreationDTO>
    {
    }
}
