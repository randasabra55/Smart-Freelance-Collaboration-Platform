using Microsoft.AspNetCore.Mvc;
using Smart_Freelance_API.Bases;
using Smart_Freelance_Core.Features.Proposals.Commands.SubmitProposal;
using Smart_Freelance_Core.Features.Proposals.Commands.UpdateProposalStatus;

namespace Smart_Freelance_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProposalController : AppControllerBase
    {
        [HttpPost("Submit")]
        public async Task<IResult> SubmitProposal([FromBody] SubmitProposalCommand command)
        {
            var result = await Mediator.Send(command);
            if (!result.IsSuccess)
                return Results.BadRequest(result.Error);
            return Results.Ok(result.Data);
        }
        /////////////////////////////////////////////
        [HttpPut("UpdateStatus")]
        public async Task<IResult> UpdateStatus([FromBody] UpdateProposalStatusCommand command)
        {
            var result = await Mediator.Send(command);
            if (!result.IsSuccess)
                return Results.BadRequest(result.Error);
            return Results.Ok(result.Data);
        }
        //////////////////////////////////

    }
}
