
using FluentValidation;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Utility;

namespace Smart_Freelance_Core.Features.Auth.Commands.ConfirmChangeEmail;
internal class ConfirmChangeEmailValidator : AbstractValidator<ConfirmChangeEmailCommand>
{
    public ConfirmChangeEmailValidator()
    {
        RuleFor(x => x.VerificationCode)
            .NotEmpty()
            .Length(6);

        RuleFor(x => x.Email).NotEmpty()
            .WithMessage(ErrorCode.RequiredFieldMissing.ToString())
            .Matches(RegexTemplates.Email)
            .WithErrorCode(ErrorCode.InvalidEmailFormat.ToString());
    }
}
