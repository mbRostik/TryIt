using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.UseCases.Commands;
using Users.Domain.Entities;
using Users.Infrastructure.Data;

namespace Users.Application.UseCases.Handlers.OperationHandlers
{
    public class PostCreatedHandler : IRequestHandler<CreatePostCommand, Post>
    {
        private readonly IMediator mediator;

        private readonly UserDbContext dbContext;
        public readonly Serilog.ILogger _logger;


        public PostCreatedHandler(UserDbContext dbContext, IMediator mediator, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            _logger = logger;
        }

        public async Task<Post> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            _logger.Information("Attempting to create a new post");

            try
            {
                var model = await dbContext.Posts.AddAsync(request.model, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.Information("Post created successfully with ID {PostId}", model.Entity.Id);

                return model.Entity;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while creating a new post");

                return null;
            }
        }
    }
}