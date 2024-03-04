using MassTransit;
using MediatR;
using MessageBus.Messages.PostService;
using Posts.Application.UseCases.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.UseCases.Handlers.NotifyHandlers
{
    public class PostCreatedNotificationHandler : INotificationHandler<PostCreatedNotification>
    {
        private readonly IPublishEndpoint _publisher;

        public PostCreatedNotificationHandler(
           IPublishEndpoint publisher)
        {
            _publisher = publisher;
        }

        public async Task Handle(PostCreatedNotification notification, CancellationToken cancellationToken)
        {
            PostCreatedEvent productCreatedEvent = new PostCreatedEvent();
            productCreatedEvent.PostId=notification.item.Id;
            Console.WriteLine("Publishing PostCreatedEvent PostId: " + productCreatedEvent.PostId);
            await _publisher.Publish(productCreatedEvent);
        }
    }
}