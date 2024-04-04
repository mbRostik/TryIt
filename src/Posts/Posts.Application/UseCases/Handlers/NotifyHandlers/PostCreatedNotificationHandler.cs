using MassTransit;
using MediatR;
using MessageBus.Messages.Events.PostService;
using MessageBus.Models.DTOs;
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
                PostCreationDTO creationEvent = new PostCreationDTO
                {
                    PostId = notification.item.Id,
                    CreatorId = notification.item.UserId,
                    Status = MessageBus.Models.Statuses.PostCreationStatuses.PostWebApi_Created
                };

                await _publisher.Publish<IPostCreate_SendEvent_From_PostWebApi>(new { CorrelationId = Guid.NewGuid(), Data = creationEvent });


                logger.Information("Publishing PostCreatedEvent PostId: {PostId}", creationEvent.PostId);

                logger.Information("PostCreatedEvent for PostId: {PostId} published successfully", creationEvent.PostId);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error handling PostCreatedNotification for PostId: {PostId}", notification.item.Id);
            }
        }
    }
}