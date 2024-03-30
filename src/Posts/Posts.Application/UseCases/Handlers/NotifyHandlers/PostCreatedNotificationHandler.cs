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
        private readonly Serilog.ILogger logger;

        public PostCreatedNotificationHandler(
           IPublishEndpoint publisher, Serilog.ILogger logger)
        {
            _publisher = publisher;
            this.logger = logger;
        }

        public async Task Handle(PostCreatedNotification notification, CancellationToken cancellationToken)
        {
            logger.Information("Handling PostCreatedNotification for PostId: {PostId}", notification.item.Id);

            try
            {
                PostCreatedEvent postCreatedEvent = new PostCreatedEvent
                {
                    PostId = notification.item.Id
                };

                logger.Information("Publishing PostCreatedEvent PostId: {PostId}", postCreatedEvent.PostId);
                await _publisher.Publish(postCreatedEvent);

                logger.Information("PostCreatedEvent for PostId: {PostId} published successfully", postCreatedEvent.PostId);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error handling PostCreatedNotification for PostId: {PostId}", notification.item.Id);
            }
        }
    }
}