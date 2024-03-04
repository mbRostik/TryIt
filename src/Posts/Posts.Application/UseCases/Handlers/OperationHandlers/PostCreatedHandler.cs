using MediatR;
using Microsoft.EntityFrameworkCore;
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

        public PostCreatedHandler(PostDbContext dbContext, IMediator mediator)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
        }

        public async Task<Post> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {

            //try
            //{
            //    using (var transaction = dbContext.Database.BeginTransaction())
            //    {
            //        try
            //        {
            //            var model = await dbContext.Posts.AddAsync(request.model);

            //            await dbContext.SaveChangesAsync();

            //            Console.WriteLine("The post was created and the notification was sent");
            //            await mediator.Publish(new PostCreatedNotification(model.Entity), cancellationToken);

            //            return model.Entity;
            //        }
            //        catch (Exception)
            //        {
            //            transaction.Rollback();
            //            return null;
            //        }
            //    }

            //}

            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //    return null;
            //}
            try
            {
                var model = await dbContext.Posts.AddAsync(request.model);

                await dbContext.SaveChangesAsync();

                Console.WriteLine("The post was created and the notification was sent");
                await mediator.Publish(new PostCreatedNotification(model.Entity), cancellationToken);

                return model.Entity;
            }




            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
