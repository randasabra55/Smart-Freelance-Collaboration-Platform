using FluentValidation;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Utility;

namespace Smart_Freelance_Core.Features.Auth.Commands.ConfirmEmailFeature;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithErrorCode(ErrorCode.InvalidInput.ToString())
            .Matches(RegexTemplates.Email).WithErrorCode(ErrorCode.InvalidEmailFormat.ToString());


        /* RuleFor(x => x.VerificationCode)
             .NotEmpty().WithErrorCode(ErrorCode.InvalidInput.ToString())
             .Matches(RegexTemplates.Otp).WithErrorCode(ErrorCode.InvalidToken.ToString());*/
    }

}
