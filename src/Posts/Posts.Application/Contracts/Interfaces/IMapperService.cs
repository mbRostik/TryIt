using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.Contracts.Interfaces
{
    public interface IMapperService
    {
        IMapper InitializeAutomapper_Post_To_GiveProfilePostDTO();
        IMapper InitializeAutomapper_CreatePostDTO_To_Post();
    }
}
