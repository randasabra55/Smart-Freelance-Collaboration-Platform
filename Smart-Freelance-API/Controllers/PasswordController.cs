using Microsoft.AspNetCore.Mvc;
using Smart_Freelance_API.Bases;
using Smart_Freelance_Core.Passwords.Commands.ChangePassword;
using Smart_Freelance_Core.Passwords.Commands.ResetPassword;
using Smart_Freelance_Core.Passwords.Commands.VerifyOTP;

namespace Smart_Freelance_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : AppControllerBase
    {
        [HttpPatch("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("SendResetPasswordOtp")]
        public async Task<IActionResult> SendResetPasswordOtp([FromBody] SendResetPasswordOTP command)
        {
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp([FromBody] verifyOTPCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPatch("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
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
