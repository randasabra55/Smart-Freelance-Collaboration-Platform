using Microsoft.AspNetCore.Mvc;
using Smart_Freelance_API.Bases;
using Smart_Freelance_Core.Features.Auth.Commands.ChangeEmail;
using Smart_Freelance_Core.Features.Auth.Commands.ConfirmChangeEmail;
using Smart_Freelance_Core.Features.Auth.Commands.ConfirmEmailFeature;
using Smart_Freelance_Core.Features.Auth.Commands.LoginFeature.Handler;
using Smart_Freelance_Core.Features.Auth.Commands.LogoutFeature;
using Smart_Freelance_Core.Features.Auth.Commands.RefreshTokenFeature.Handler;
using Smart_Freelance_Core.Features.Auth.Commands.RegisterFeature.Handler;
using Smart_Freelance_Core.Features.Auth.Commands.SendEmailConfirmationFeature.Handler;

namespace Smart_Freelance_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : AppControllerBase
    {
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPatch("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await Mediator.Send(new LogoutCommand());
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPatch("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("SendConfirmationEmail")]
        public async Task<IActionResult> SendConfirmationEmail([FromQuery] string email)
        {
            var command = new SendEmailConfirmationCommand { Email = email };
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPatch("RequestChangeEmail")]
        public async Task<IActionResult> RequestChangeEmail([FromBody] ChangeEmailCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPatch("ConfirmChangeEmail")]
        //[Authorize]
        public async Task<IActionResult> ConfirmChangeEmail([FromBody] ConfirmChangeEmailCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
