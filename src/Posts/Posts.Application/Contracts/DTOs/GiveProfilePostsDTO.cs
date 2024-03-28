using Posts.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.Contracts.DTOs
{
    public class GiveProfilePostsDTO
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public List<PFile> files { get; set; }

    }
}
