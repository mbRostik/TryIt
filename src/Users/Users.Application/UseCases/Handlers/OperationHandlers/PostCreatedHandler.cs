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

        public PostCreatedHandler(UserDbContext dbContext, IMediator mediator)
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

            //            dbContext.SaveChanges();
            //            transaction.Commit();
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