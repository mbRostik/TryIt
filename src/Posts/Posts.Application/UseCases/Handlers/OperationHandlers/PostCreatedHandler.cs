using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Application.Contracts.Interfaces;
using Posts.Application.UseCases.Commands;
using Posts.Application.UseCases.Notifications;
using Posts.Domain.Entities;
using Posts.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.UseCases.Handlers.Creation
{
    public class PostCreatedHandler : IRequestHandler<CreatePostCommand, bool>
    {
        private readonly IMediator mediator;

        private readonly PostDbContext dbContext;
        private readonly IMapperService _mapper;
        private readonly Serilog.ILogger logger;

        public PostCreatedHandler(PostDbContext dbContext, IMediator mediator, IMapperService mapper, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._mapper = mapper;
            this.logger = logger;
        }

        public async Task<bool> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            var mapper = _mapper.InitializeAutomapper_CreatePostDTO_To_Post();

            try
            {
                if (request.model.UserId == null || (request.model.Content == null && request.model.Title == null))
                {
                    return false;
                }
                Post temp = mapper.Map<Post>(request.model);
                var model = await dbContext.Posts.AddAsync(temp);
                await dbContext.SaveChangesAsync();

                logger.Information("Post with ID {PostId} created successfully", model.Entity.Id);
                await mediator.Publish(new PostCreatedNotification(model.Entity), cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error creating post. {ErrorMessage}", ex.Message);
                return false;
            }
        }
    }
}
