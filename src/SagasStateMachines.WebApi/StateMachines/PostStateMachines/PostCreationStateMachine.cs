using MassTransit;
using MessageBus.Messages.Events.IdentityServerService;
using MessageBus.Messages;
using MessageBus.Models.DTOs;
using SagasStateMachines.WebApi.States.UserStates;
using SagasStateMachines.WebApi.States.PostStates;
using MessageBus.Messages.Events.PostService;
using MessageBus.Messages.Commands.IdentityServerService;
using MessageBus.Messages.Commands.PostService;
using MessageBus.Models.Statuses;
using SagasStateMachines.WebApi.Events.UserEvents;
using SagasStateMachines.WebApi.Events.PostEvents;

namespace SagasStateMachines.WebApi.StateMachines.PostStateMachines
{
    public class PostCreationStateMachine : MassTransitStateMachine<ProcessingPostCreationState>
    {
        private readonly Serilog.ILogger _logger;

        public PostCreationStateMachine(Serilog.ILogger logger)
        {
            this.InstanceState(x => x.State);
            this.State(() => (MassTransit.State)this.Processing);
            this.ConfigureCorrelationIds();
            this.Initially(this.SetPostSubmitted_FromPostService_Handler());
            this.During((MassTransit.State)Processing, this.SetPostSubmitted_FromUserService_Handler(),
                this.SetPostSubmitted_FromReportService_Handler());
            SetCompletedWhenFinalized();
            _logger = logger;
        }
        public MassTransit.State Processing { get; private set; }

        public Event<IPostCreate_Cancelled> PostCreate_Cancelled { get; private set; }
        public Event<IPostCreate_SendEvent_From_UserWebApi> PostCreate_SendEvent_From_UserWebApi { get; private set; }
        public Event<IPostCreate_SendEvent_From_PostWebApi> PostCreate_SendEvent_From_PostWebApi { get; private set; }
        public Event<IPostCreate_SendEvent_From_ReportWebApi> PostCreate_SendEvent_From_ReportWebApi { get; private set; }

        private void ConfigureCorrelationIds()
        {
            this.Event(() => this.PostCreate_SendEvent_From_PostWebApi, x => x.CorrelateById(c => c.Message.CorrelationId).SelectId(c => c.Message.CorrelationId));
            this.Event(() => this.PostCreate_SendEvent_From_UserWebApi, x => x.CorrelateById(c => c.Message.CorrelationId));
            this.Event(() => this.PostCreate_SendEvent_From_ReportWebApi, x => x.CorrelateById(c => c.Message.CorrelationId));
        }



        private EventActivityBinder<ProcessingPostCreationState, IPostCreate_SendEvent_From_PostWebApi> SetPostSubmitted_FromPostService_Handler() =>
          When(PostCreate_SendEvent_From_PostWebApi).Then(c => this.UpdateSagaState(c.Instance, c.Data.Data))
                              .Then(c => _logger.Information($"PostCreation submitted to {c.Data.CorrelationId} UserWebApi"))
                              .ThenAsync(c => this.SendCommand<IPostCreate_Send_To_UserWebApi>("rabbitmq://localhost/rabbitUserWebApiQueue", c))
                                .TransitionTo((MassTransit.State)Processing);

        private EventActivityBinder<ProcessingPostCreationState, IPostCreate_SendEvent_From_UserWebApi> SetPostSubmitted_FromUserService_Handler() =>
          When(PostCreate_SendEvent_From_UserWebApi).Then(c => this.UpdateSagaState(c.Instance, c.Data.Data))
                              .Then(c => _logger.Information($"PostCreation submitted to {c.Data.CorrelationId} ReportWebApi"))
                              .ThenAsync(c => this.SendCommand<IPostCreate_Send_To_ReportWebApi>("rabbitmq://localhost/rabbitReportWebApiQueue", c));


        private EventActivityBinder<ProcessingPostCreationState, IPostCreate_SendEvent_From_ReportWebApi> SetPostSubmitted_FromReportService_Handler() =>
           When(PostCreate_SendEvent_From_ReportWebApi).Then(c =>
           {
               this.UpdateSagaState(c.Instance, c.Data.Data);
               c.Instance.Data.Status = PostCreationStatuses.ReportWebApi_Created;
           })
                             .Publish(c => new PostCreatedProcessed(c.Data.CorrelationId, c.Data.Data))
                             .Then(c => _logger.Information($"PostCreation finalization {c.Data.Data.PostId}"))
                             .Finalize();


        private void UpdateSagaState(ProcessingPostCreationState state, PostCreationDTO post)
        {
            var currentDate = DateTime.Now;
            state.Created = currentDate;
            state.Updated = currentDate;
            state.Data = post;
        }

        private async Task SendCommand<TCommand>(string endpointKey, BehaviorContext<ProcessingPostCreationState, IMessage<PostCreationDTO>> context)
           where TCommand : class, IMessage<PostCreationDTO>
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
