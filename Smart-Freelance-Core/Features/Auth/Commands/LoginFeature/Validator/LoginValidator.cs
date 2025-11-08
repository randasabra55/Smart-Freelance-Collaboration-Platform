using FluentValidation;
using Smart_Freelance_Core.Features.Auth.Commands.LoginFeature.Handler;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Utility;

namespace Smart_Freelance_Core.Features.Auth.Commands.LoginFeature.Validator;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithErrorCode(ErrorCode.InvalidInput.ToString())
            .Matches(RegexTemplates.Email).WithErrorCode(ErrorCode.InvalidEmailFormat.ToString());

        RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode(ErrorCode.InvalidInput.ToString())
            .MinimumLength(8).WithErrorCode(ErrorCode.WeakPassword.ToString())
            .Matches(RegexTemplates.Password)
            .WithErrorCode(ErrorCode.WeakPassword.ToString());
    }

}
