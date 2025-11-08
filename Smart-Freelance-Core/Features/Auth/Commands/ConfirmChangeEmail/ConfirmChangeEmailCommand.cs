using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Service.Abstracts;


namespace Smart_Freelance_Core.Features.Auth.Commands.ConfirmChangeEmail;

//[Authorize]
//[Endpoint(EndpointMethod.Patch, EndpointTag.Email, "ConfirmChangeEmail")]
public record ConfirmChangeEmailCommand(string Email, string VerificationCode) : IRequest<Result<string>>;
public class ConfirmChangeEmailCommandHandler(
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    IEmailConfirmationService emailConfirmationService)
    : IRequestHandler<ConfirmChangeEmailCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ConfirmChangeEmailCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = httpContextAccessor.HttpContext!.User
               .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        long? userId = null;

        if (long.TryParse(userIdClaim, out var parsedId))
        {
            userId = parsedId;
        }
        var user = await userManager.FindByIdAsync(userId.ToString()!);
        if (user == null)
            return ErrorCode.UserNotFound;

        return await emailConfirmationService.ConfirmChangeEmail(user, request.Email, request.VerificationCode);
    }
}
