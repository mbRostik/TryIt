using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Models.Statuses
{
    [Serializable]
    public enum UserChangeStatus
    {
        ChatWebApi_Changed,
        UserWebApi_Changed
    }
}
