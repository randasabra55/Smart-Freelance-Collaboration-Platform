using FluentValidation;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Utility;

namespace Smart_Freelance_Core.Features.Auth.Commands.ChangeEmail;
public class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
{
    public ChangeEmailCommandValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .WithMessage(ErrorCode.RequiredFieldMissing.ToString())
            .Matches(RegexTemplates.Email)
            .WithErrorCode(ErrorCode.InvalidEmailFormat.ToString());
        RuleFor(x => x.CurrentEmail)
            .NotEmpty()
            .WithMessage(ErrorCode.RequiredFieldMissing.ToString())
            .Matches(RegexTemplates.Email)
            .WithErrorCode(ErrorCode.InvalidEmailFormat.ToString());
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage(ErrorCode.RequiredFieldMissing.ToString());

    }
}
