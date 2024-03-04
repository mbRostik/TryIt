using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reports.Application.UseCases.Queries;
using Reports.Domain;

namespace Reports.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMediator mediator;

        public ReportController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("GetAllPosts")]
        public async Task<ActionResult<List<Post>>> GetItems()
        {

            var result = await mediator.Send(new GetAllPostsQuery());

            return Ok(result);

        }
    }
}
