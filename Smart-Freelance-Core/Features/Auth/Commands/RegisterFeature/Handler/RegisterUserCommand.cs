

using MediatR;
using Smart_Freelance_Core.Features.Auth.Commands.RegisterFeature.Extensions;
using Smart_Freelance_Core.Features.Auth.Dto;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Helpers;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Service.Abstracts;

namespace Smart_Freelance_Core.Features.Auth.Commands.RegisterFeature.Handler;

//[Endpoint(EndpointMethod.Post, EndpointTag.Auth, "Register")]
public record RegisterUserCommand : IRequest<Result>
{
    public required CreateUserRequest userRequest { get; set; }

}

public class RegisterCommandHandler(
    IIdentityService identityService,
    IEmailConfirmationService emailConfirmationService
    )

    : IRequestHandler<RegisterUserCommand, Result>
{
    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingEmail = await identityService.IsUserEmailExist(request.userRequest.Email);
        if (existingEmail)
        {
            return ErrorCode.EmailAlreadyExists;
        }

        if (request.userRequest.PhoneNumber.HasValue())
        {
            var existingPhone = await identityService.IsUserPhoneNumExist(request.userRequest.PhoneNumber!);

            if (existingPhone)
            {
                return ErrorCode.PhoneNumberAlreadyExists;
            }
        }
        // Create user
        var creationResult = await identityService.CreateUserAsync(request.userRequest.ToEntity(), request.userRequest.Password);

        if (!creationResult.IsSuccess)
        {
            return creationResult.Error;
        }

        // Send confirmation email
        await emailConfirmationService.SendEmailConfirmation(creationResult.Data);

        return Result.Success();
    }
}
