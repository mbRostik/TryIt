using MassTransit;
using MessageBus.Messages;
using MessageBus.Messages.Commands.IdentityServerService;
using MessageBus.Messages.Events.IdentityServerService;
using MessageBus.Models.DTOs;
using MessageBus.Models.Statuses;
using SagasStateMachines.WebApi.Events.UserEvents;
using SagasStateMachines.WebApi.States.UserStates;

namespace SagasStateMachines.WebApi.StateMachines.UserStateMachines
{
    public class UserCreationStateMachine : MassTransitStateMachine<ProcessingUserCreationState>
    {
        private readonly Serilog.ILogger _logger;

        public UserCreationStateMachine(Serilog.ILogger logger)
        {
            this.InstanceState(x => x.State);
            this.State(() => (MassTransit.State)this.Processing);
            this.ConfigureCorrelationIds();
            this.Initially(this.SetUserSubmitted_FromIdentityServer_Handler());
            this.During((MassTransit.State)Processing, this.SetUserSubmitted_FromUserService_Handler(), 
                this.SetUserSubmitted_FromChatService_Handler(), 
                this.SetUserSubmitted_FromPostService_Handler(),
                this.SetUserSubmitted_FromNotificationService_Handler(),
                this.SetUserSubmitted_FromReportService_Handler(),
                this.SetUserSubmitted_FromSubscriptionService_Handler());
            SetCompletedWhenFinalized();
            _logger=logger;
        }
        public MassTransit.State Processing { get; private set; }

        public Event<IUserCreate_Cancelled> UserCreate_Cancelled { get; private set; }
        public Event<IUserCreate_SendEvent_From_IdentityServer> UserCreate_SendEvent_From_IdentityServer { get; private set; }
        public Event<IUserCreate_SendEvent_From_UserWebApi> UserCreate_SendEvent_From_UserWebApi { get; private set; }
        public Event<IUserCreate_SendEvent_From_ChatWebApi> UserCreate_SendEvent_From_ChatWebApi { get; private set; }
        public Event<IUserCreate_SendEvent_From_PostWebApi> UserCreate_SendEvent_From_PostWebApi { get; private set; }
        public Event<IUserCreate_SendEvent_From_ReportWebApi> UserCreate_SendEvent_From_ReportWebApi { get; private set; }
        public Event<IUserCreate_SendEvent_From_SubscriptionWebApi> UserCreate_SendEvent_From_SubscriptionWebApi { get; private set; }
        public Event<IUserCreate_SendEvent_From_NotificationWebApi> UserCreate_SendEvent_From_NotificationWebApi { get; private set; }

        private void ConfigureCorrelationIds()
        {
            this.Event(() => this.UserCreate_SendEvent_From_IdentityServer, x => x.CorrelateById(c => c.Message.CorrelationId).SelectId(c => c.Message.CorrelationId));
            this.Event(() => this.UserCreate_SendEvent_From_UserWebApi, x => x.CorrelateById(c => c.Message.CorrelationId));
            this.Event(() => this.UserCreate_SendEvent_From_ChatWebApi, x => x.CorrelateById(c => c.Message.CorrelationId));
            this.Event(() => this.UserCreate_SendEvent_From_PostWebApi, x => x.CorrelateById(c => c.Message.CorrelationId));
            this.Event(() => this.UserCreate_SendEvent_From_NotificationWebApi, x => x.CorrelateById(c => c.Message.CorrelationId));
            this.Event(() => this.UserCreate_SendEvent_From_ReportWebApi, x => x.CorrelateById(c => c.Message.CorrelationId));
            this.Event(() => this.UserCreate_SendEvent_From_SubscriptionWebApi, x => x.CorrelateById(c => c.Message.CorrelationId));
        }


        private EventActivityBinder<ProcessingUserCreationState, IUserCreate_SendEvent_From_IdentityServer> SetUserSubmitted_FromIdentityServer_Handler() =>
          When(UserCreate_SendEvent_From_IdentityServer).Then(c => this.UpdateSagaState(c.Instance, c.Data.Data))
                              .Then(c => _logger.Information($"UserCreation submitted to {c.Data.CorrelationId} UserWebApi"))
                              .ThenAsync(c => this.SendCommand<IUserCreate_Send_To_UserWebApi>("rabbitmq://localhost/rabbitUserWebApiQueue", c))
                              .TransitionTo((MassTransit.State)Processing);

        private EventActivityBinder<ProcessingUserCreationState, IUserCreate_SendEvent_From_UserWebApi> SetUserSubmitted_FromUserService_Handler() =>
          When(UserCreate_SendEvent_From_UserWebApi).Then(c => this.UpdateSagaState(c.Instance, c.Data.Data))
                              .Then(c => _logger.Information($"UserCreation submitted to {c.Data.CorrelationId} ChatWebApi"))
                              .ThenAsync(c => this.SendCommand<IUserCreate_Send_To_ChatWebApi>("rabbitmq://localhost/rabbitChatWebApiQueue", c));

        private EventActivityBinder<ProcessingUserCreationState, IUserCreate_SendEvent_From_ChatWebApi> SetUserSubmitted_FromChatService_Handler() =>
          When(UserCreate_SendEvent_From_ChatWebApi).Then(c => this.UpdateSagaState(c.Instance, c.Data.Data))
                              .Then(c => _logger.Information($"UserCreation submitted to {c.Data.CorrelationId} PostWebApi"))
                              .ThenAsync(c => this.SendCommand<IUserCreate_Send_To_PostWebApi>("rabbitmq://localhost/rabbitPostWebApiQueue", c));

        private EventActivityBinder<ProcessingUserCreationState, IUserCreate_SendEvent_From_PostWebApi> SetUserSubmitted_FromPostService_Handler() =>
          When(UserCreate_SendEvent_From_PostWebApi).Then(c => this.UpdateSagaState(c.Instance, c.Data.Data))
                              .Then(c => _logger.Information($"UserCreation submitted to {c.Data.CorrelationId} NotificationWebApi"))
                              .ThenAsync(c => this.SendCommand<IUserCreate_Send_To_NotificationWebApi>("rabbitmq://localhost/rabbitNotificationWebApiQueue", c));

        private EventActivityBinder<ProcessingUserCreationState, IUserCreate_SendEvent_From_NotificationWebApi> SetUserSubmitted_FromNotificationService_Handler() =>
          When(UserCreate_SendEvent_From_NotificationWebApi).Then(c => this.UpdateSagaState(c.Instance, c.Data.Data))
                              .Then(c => _logger.Information($"UserCreation submitted to {c.Data.CorrelationId} Report"))
                              .ThenAsync(c => this.SendCommand<IUserCreate_Send_To_ReportWebApi>("rabbitmq://localhost/rabbitReportWebApiQueue", c));

        private EventActivityBinder<ProcessingUserCreationState, IUserCreate_SendEvent_From_ReportWebApi> SetUserSubmitted_FromReportService_Handler() =>
          When(UserCreate_SendEvent_From_ReportWebApi).Then(c => this.UpdateSagaState(c.Instance, c.Data.Data))
                              .Then(c => _logger.Information($"UserCreation submitted to {c.Data.CorrelationId} Subscription"))
                              .ThenAsync(c => this.SendCommand<IUserCreate_Send_To_SubscriptionWebApi>("rabbitmq://localhost/rabbitSubscriptionWebApiQueue", c));
        private EventActivityBinder<ProcessingUserCreationState, IUserCreate_SendEvent_From_SubscriptionWebApi> SetUserSubmitted_FromSubscriptionService_Handler() =>
           When(UserCreate_SendEvent_From_SubscriptionWebApi).Then(c =>
           {
               this.UpdateSagaState(c.Instance, c.Data.Data);
               c.Instance.Data.Status = UserCreationStatuses.SubscriptionWebApi_Created;
           })
                             .Publish(c => new UserCreatedProcessed(c.Data.CorrelationId, c.Data.Data))
                             .Then(c => _logger.Information($"UserCreation finalization {c.Data.Data.UserName}"))
                             .Finalize();


        private void UpdateSagaState(ProcessingUserCreationState state, UserCreationDTO user)
        {
            var currentDate = DateTime.Now;
            state.Created = currentDate;
            state.Updated = currentDate;
            state.Data = user;
        }

        private async Task SendCommand<TCommand>(string endpointKey, BehaviorContext<ProcessingUserCreationState, IMessage<UserCreationDTO>> context)
           where TCommand : class, IMessage<UserCreationDTO>
        {
            var sendEndpoint = await context.GetSendEndpoint(new Uri(endpointKey));
            await sendEndpoint.Send<TCommand>(new
            {
                CorrelationId = context.Data.CorrelationId,
                Data = context.Data.Data
            });
        }

    }
}
