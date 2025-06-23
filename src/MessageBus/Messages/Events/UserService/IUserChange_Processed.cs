using MessageBus.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Messages.Events.UserService
{
    public interface IUserChange_Processed : IMessage<UserChangeDTO>
    {
    }
}
