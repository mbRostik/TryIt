using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Messages
{
    public interface IMessage<T>
    {
        Guid CorrelationId { get; }
        T Data { get; }
    }
}
    