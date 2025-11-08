using FluentValidation;
using Smart_Freelance_Infrastructure.Common.Utility;

namespace Smart_Freelance_Core.Passwords.Commands.ResetPassword;
public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .Matches(RegexTemplates.Email)
            .WithMessage("Invalid email format.");


        RuleFor(x => x.OTP)
            .NotEmpty()
            .WithMessage("OTP is required.")
            .Length(6)
            .WithMessage("OTP must be 6 digits long.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")
            .Matches(RegexTemplates.Password)
            .WithMessage("Invalid Password Format.");



    }
}
