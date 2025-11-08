using FluentValidation;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Utility;

namespace Smart_Freelance_Core.Passwords.Commands.ChangePassword;
public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithMessage("Old password is required.")
            .Matches(RegexTemplates.Password)
            .WithMessage("Invalid Password Format.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")
            .Matches(RegexTemplates.Password)
            .WithMessage("Invalid Password Format.");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty()
            .WithMessage("Confirm password is required.")
            .Equal(x => x.NewPassword)
            .WithErrorCode(ErrorCode.PasswordMismatch.ToString());
    }
}
