using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Models.Statuses
{
    [Serializable]
    public enum UserCreationStatuses
    {
        IdentityServer_Created,
        UserWebApi_Created,
        ChatWebApi_Created,
        PostWebApi_Created,
        ReportWebApi_Created,
        NotificationWebApi_Created,
        SubscriptionWebApi_Created
    }
}
