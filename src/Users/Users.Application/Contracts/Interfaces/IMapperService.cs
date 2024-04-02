using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Application.Contracts.Interfaces
{
    public interface IMapperService
    {
        IMapper Mapper_UserChatProfileToGiveUserForChat();
        IMapper Mapper_UserToUserProfileDTO();
        IMapper Mapper_ChangeUserProfileToUserDTO();

        IMapper Mapper_UserToUserChatProfileDTO ();
    }
}
