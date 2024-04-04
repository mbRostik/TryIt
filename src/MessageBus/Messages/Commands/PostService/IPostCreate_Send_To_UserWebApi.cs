using MessageBus.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Messages.Commands.PostService
{
    public interface IPostCreate_Send_To_UserWebApi : IMessage<PostCreationDTO>
    {
    }
}
