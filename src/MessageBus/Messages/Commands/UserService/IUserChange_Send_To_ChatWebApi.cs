using MessageBus.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Messages.Commands.UserService
{
    public interface IUserChange_Send_To_ChatWebApi : IMessage<UserChangeDTO>
    {
    }
}
