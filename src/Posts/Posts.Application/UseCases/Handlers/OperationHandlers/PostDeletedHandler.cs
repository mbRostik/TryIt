using MediatR;
using Microsoft.EntityFrameworkCore;
using Posts.Application.Contracts.Interfaces;
using Posts.Application.UseCases.Commands;
using Posts.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Application.UseCases.Handlers.OperationHandlers
{
    public class PostDeletedHandler : IRequestHandler<DeletePostCommand, bool>
    {
        private readonly IMediator mediator;

        private readonly PostDbContext dbContext;
        private readonly IMapperService _mapper;
        private readonly Serilog.ILogger logger;

        public PostDeletedHandler(PostDbContext dbContext, IMediator mediator, IMapperService mapper, Serilog.ILogger logger)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
            this._mapper = mapper;
            this.logger = logger;
        }

        public async Task<bool> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.Information("Deleting post and its files for postId: {PostId} and userId: {UserId}", request.postId, request.userId);

                var isUserPost = await dbContext.Posts
                      .FirstOrDefaultAsync(x => x.UserId == request.userId && x.Id == request.postId);

                if (isUserPost == null)
                {
                    logger.Warning("Post with postId: {PostId} and userId: {UserId} not found.", request.postId, request.userId);
                    return false;
                }

                var pFiles = await dbContext.PFiles
                    .Where(x => x.PostId == request.postId)
                    .ToListAsync();

                dbContext.PFiles.RemoveRange(pFiles);
                dbContext.Posts.Remove(isUserPost);

                await dbContext.SaveChangesAsync();

                logger.Information("Successfully deleted post and its files for postId: {PostId} and userId: {UserId}", request.postId, request.userId);

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error occurred while deleting post and its files for postId: {PostId} and userId: {UserId}", request.postId, request.userId);
                return false;
            }
        }

    }
}
