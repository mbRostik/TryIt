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
    public class PostCreatedHandler : IRequestHandler<CreatePostCommand, Post>
    {
        private readonly IMediator mediator;

        private readonly PostDbContext dbContext;
        private readonly IMapperService _mapper;

        public PostCreatedHandler(PostDbContext dbContext, IMediator mediator, IMapperService mapper)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._mapper = mapper;
        }

        public async Task<Post> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            var mapper = _mapper.InitializeAutomapper_CreatePostDTO_To_Post();

            try
            {
                Post temp = mapper.Map<Post>(request.model);

                var model = await dbContext.Posts.AddAsync(temp);
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                await mediator.Publish(new PostCreatedNotification(model.Entity), cancellationToken);

                return model.Entity;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(ex.ToString());
                return null; 
            }
        }
    }
}
