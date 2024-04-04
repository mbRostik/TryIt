using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Models.Statuses
{

    [Serializable]
    public enum PostCreationStatuses
    {
        UserWebApi_Created,
        PostWebApi_Created,
        ReportWebApi_Created
    }
}
