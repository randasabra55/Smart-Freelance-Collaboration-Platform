using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Smart_Freelance_Data.Entities.Identity;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Service.Abstracts;

namespace Smart_Freelance_Core.Features.Auth.Commands.ChangeEmail;

//[Authorize]
//[Endpoint(EndpointMethod.Patch, EndpointTag.Email, "RequestChangeEmail")]
public record ChangeEmailCommand(string CurrentEmail, string CurrentPassword, string NewEmail) : IRequest<Result>;

public class ChangeEmailCommandHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IEmailConfirmationService emailConfirmationService, IIdentityService identityService) : IRequestHandler<ChangeEmailCommand, Result>
{
    public async Task<Result> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
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
        ////////////////////////////////////////////////////////////////
        if (!user.Email!.ToLower().Equals(request.CurrentEmail.ToLower()))
            return ErrorCode.InvalidEmail;

        var passwordValid = await userManager.CheckPasswordAsync(user, request.CurrentPassword);
        if (!passwordValid)
            return ErrorCode.IncorrectPassword;

        //////////////////////////////////////////////////////////////////
        if (user.Email!.ToLower() == request.NewEmail.ToLower())
            return ErrorCode.SameEmail;

        if (await identityService.IsUserEmailExist(request.NewEmail))
            return ErrorCode.EmailAlreadyExists;

        await emailConfirmationService.SendChangeEmailOtp(request.NewEmail, user);


        return Result.Success();
    }
}
