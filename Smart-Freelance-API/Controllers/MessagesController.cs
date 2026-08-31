using Microsoft.AspNetCore.Mvc;
using Smart_Freelance_API.Bases;
using Smart_Freelance_Core.Features.Messages.Queries.GetMessages;

namespace Smart_Freelance_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : AppControllerBase
    {
        [HttpGet("GetMessages")]
        public async Task<IResult> GetMessages([FromQuery] GetMessagesQuery query)
        {
            var result = await Mediator.Send(query);
            if (!result.IsSuccess)
                return Results.BadRequest(result.Error);
            return Results.Ok(result.Data);
        }

        ////////////////////////////////////////////////////////
        [HttpGet]
        public async Task<IResult> SetMessages([FromQuery] GetMessagesQuery query)
        {
            var result = await Mediator.Send(query);
            if (!result.IsSuccess)
                return Results.BadRequest(result.Error);
            return Results.Ok(result.Data);
        }
    }
}
