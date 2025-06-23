using MassTransit;
using MessageBus.Messages;
using MessageBus.Messages.Commands.UserService;
using MessageBus.Messages.Events.UserService;
using MessageBus.Models.DTOs;
using SagasStateMachines.WebApi.Events.UserEvents;
using SagasStateMachines.WebApi.States.UserStates;

namespace SagasStateMachines.WebApi.StateMachines.UserStateMachines
{
    public class UserChangeStateMachine : MassTransitStateMachine<ProcessingUserChangeState>
    {
        private readonly Serilog.ILogger _logger;

        public UserChangeStateMachine(Serilog.ILogger logger)
        {
            this.InstanceState(x => x.State);
            this.State(() => (MassTransit.State)this.Processing);
            this.ConfigureCorrelationIds();
            this.Initially(this.SetUserChanged_FromUserWebApi_Handler());
            this.During((MassTransit.State)Processing, this.SetUserChanged_FromChatWebApi_Handler());
            SetCompletedWhenFinalized();
            _logger = logger;
        }
        public MassTransit.State Processing { get; private set; }

        public Event<IUserChange_SendEvent_From_UserWebApi> UserChange_SendEvent_From_UserWebApi { get; private set; }
        public Event<IUserChange_SendEvent_From_ChatWebApi> UserChange_SendEvent_From_ChatWebApi { get; private set; }

        private void ConfigureCorrelationIds()
        {
            this.Event(() => this.UserChange_SendEvent_From_UserWebApi, x => x.CorrelateById(c => c.Message.CorrelationId).SelectId(c => c.Message.CorrelationId));
            this.Event(() => this.UserChange_SendEvent_From_ChatWebApi, x => x.CorrelateById(c => c.Message.CorrelationId));
        }

        private EventActivityBinder<ProcessingUserChangeState, IUserChange_SendEvent_From_UserWebApi> SetUserChanged_FromUserWebApi_Handler() =>
          When(UserChange_SendEvent_From_UserWebApi).Then(c => this.UpdateSagaState(c.Instance, c.Data.Data))
                              .Then(c => _logger.Information($"UserChange submitted to {c.Data.CorrelationId} UserWebApi"))
                              .ThenAsync(c => this.SendCommand<IUserChange_Send_To_ChatWebApi>("rabbitmq://localhost/rabbitChatWebApiQueue", c))
                              .TransitionTo((MassTransit.State)Processing);
        private EventActivityBinder<ProcessingUserChangeState, IUserChange_SendEvent_From_ChatWebApi> SetUserChanged_FromChatWebApi_Handler() =>
           When(UserChange_SendEvent_From_ChatWebApi).Then(c =>
           {
               this.UpdateSagaState(c.Instance, c.Data.Data);
               c.Instance.Data.Status = MessageBus.Models.Statuses.UserChangeStatus.ChatWebApi_Changed;
           })
                             .Publish(c => new UserChangedProcessed(c.Data.CorrelationId, c.Data.Data))
                             .Then(c => _logger.Information($"UserChange finalization {c.Data.Data.UserId}"))
                             .Finalize();


        private void UpdateSagaState(ProcessingUserChangeState state, UserChangeDTO user)
        {
            var currentDate = DateTime.Now;
            state.Created = currentDate;
            state.Updated = currentDate;
            state.Data = user;
        }

        private async Task SendCommand<TCommand>(string endpointKey, BehaviorContext<ProcessingUserChangeState, IMessage<UserChangeDTO>> context)
           where TCommand : class, IMessage<UserChangeDTO>
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
