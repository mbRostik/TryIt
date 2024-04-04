using MessageBus.Models.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Models.DTOs
{
    [Serializable]

    public class PostCreationDTO
    {
        public int PostId { get; set; }

        public string CreatorId { get; set; }

        public PostCreationStatuses Status { get; set; }

    }
}
