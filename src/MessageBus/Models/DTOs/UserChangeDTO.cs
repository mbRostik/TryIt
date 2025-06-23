using MessageBus.Models.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Models.DTOs
{
    [Serializable]
    public class UserChangeDTO
    {
        public string UserId { get; set; }

        public bool IsCheckingMessages { get; set; }

        public UserChangeStatus Status { get; set; }
    }
}
