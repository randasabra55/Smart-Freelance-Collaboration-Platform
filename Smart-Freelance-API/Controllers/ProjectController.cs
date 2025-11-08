using Microsoft.AspNetCore.Mvc;
using Smart_Freelance_API.Bases;
using Smart_Freelance_Core.Features.Dashboard.Queries.ProjectPagination;
using Smart_Freelance_Core.Features.Projects.Commands.CreateProject;
using Smart_Freelance_Core.Features.Projects.Commands.DeleteProject;
using Smart_Freelance_Core.Features.Projects.Commands.ReviewProject;
using Smart_Freelance_Core.Features.Projects.Commands.SubmitProject;
using Smart_Freelance_Core.Features.Projects.Commands.UpdateProject;
using Smart_Freelance_Core.Features.Projects.Queries.GetProjectById;

namespace Smart_Freelance_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : AppControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectCommand command)
        {
            var result = await Mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);

        }
        //////////////////////////////////////////
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProject([FromBody] UpdateProjectCommand command)
        {
            var result = await Mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        ///////////////////////////////////////////////////
        [HttpDelete("{projectId:long}")]
        public async Task<IActionResult> DeleteProject(long projectId)
        {
            var result = await Mediator.Send(new DeleteProjectCommand(projectId));
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
        //////////////////////////////////////////////////////
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitProject([FromBody] SubmitProjectCommand command)
        {
            var result = await Mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        /////////////////////////////////////////////////
        [HttpPost("review-submission")]
        public async Task<IActionResult> ReviewProjectSubmission([FromBody] ReviewProjectSubmissionCommand command)
        {
            var result = await Mediator.Send(command);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        ////////////////////////////////////
        [HttpGet("GetById")]
        public async Task<IResult> GetById([FromQuery] long id)
        {
            var result = await Mediator.Send(new GetProjectByIdQuery(id));
            if (!result.IsSuccess)
                return Results.BadRequest(result.Error);
            return Results.Ok(result.Data);
        }
        //////////////////////////////////
        [HttpGet("GetProjectsPaginated")]
        public async Task<IResult> GetProjectsPaginated([FromQuery] GetProjectsPaginatedQuery query)
        {
            var result = await Mediator.Send(query);
            if (!result.IsSuccess)
                return Results.BadRequest(result.Error);
            return Results.Ok(result.Data);
        }

    }
}
