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
        void Mapper_UserToUserProfileDTO(ref IMapper mapper);
        void Mapper_ChangeUserProfileToUserDTO(ref IMapper mapper);

        void Mapper_UserToUserChatProfileDTO (ref IMapper mapper);
    }
}
