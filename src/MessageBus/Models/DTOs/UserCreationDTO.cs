using MessageBus.Models.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Models.DTOs
{
    [Serializable]

    public class UserCreationDTO
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public UserCreationStatuses Status { get; set; }

    }
}
