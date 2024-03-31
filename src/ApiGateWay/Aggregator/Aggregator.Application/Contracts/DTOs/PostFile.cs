using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aggregator.Application.Contracts.DTOs
{
    public class PostFile
    {
        public string Name { get; set; }
        public byte[] File {  get; set; }

        public int PostId { get; set; }
    }
}
