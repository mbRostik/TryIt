using MessageBus.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Messages.Events.IdentityServerService
{
    public interface IUserCreate_SendEvent_From_UserWebApi : IMessage<UserCreationDTO>
    {
    }
}
