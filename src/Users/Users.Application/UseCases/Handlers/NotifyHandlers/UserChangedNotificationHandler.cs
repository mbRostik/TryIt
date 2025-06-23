using MassTransit;
using MediatR;
using MessageBus.Messages.Events.PostService;
using MessageBus.Messages.Events.UserService;
using MessageBus.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.UseCases.Notifications;

namespace Users.Application.UseCases.Handlers.NotifyHandlers
{
    public class UserChangedNotificationHandler : INotificationHandler<UserChangedNotification>
    {
        private readonly IPublishEndpoint _publisher;
        private readonly Serilog.ILogger logger;

        public UserChangedNotificationHandler(
           IPublishEndpoint publisher, Serilog.ILogger logger)
        {
            _publisher = publisher;
            this.logger = logger;
        }

        public async Task Handle(UserChangedNotification notification, CancellationToken cancellationToken)
        {
            logger.Information("Handling UserChangedNotification for UserId: {UserId}", notification.item.Id);

            try
            {
                UserChangeDTO creationEvent = new UserChangeDTO
                {
                    UserId = notification.item.Id,
                    IsCheckingMessages = notification.item.IsCheckingMessages,
                    Status = MessageBus.Models.Statuses.UserChangeStatus.UserWebApi_Changed,
                };

                await _publisher.Publish<IUserChange_SendEvent_From_UserWebApi>(new { CorrelationId = Guid.NewGuid(), Data = creationEvent });


                logger.Information("Publishing UserChangedEvent UserId: {UserId}", creationEvent.UserId);

                logger.Information("UserChangedEvent for UserId: {UserId} published successfully", creationEvent.UserId);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error handling UserChangedNotification for UserId: {UserId}", notification.item.Id);
            }
        }
    }
}
