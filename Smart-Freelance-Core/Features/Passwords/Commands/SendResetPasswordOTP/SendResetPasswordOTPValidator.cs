using FluentValidation;
using Smart_Freelance_Infrastructure.Common.Utility;

namespace Smart_Freelance_Core.Passwords.Commands.ResetPassword;
public class SendResetPasswordOTPValidator : AbstractValidator<SendResetPasswordOTP>
{
    public SendResetPasswordOTPValidator()
    {
        RuleFor(x => x.Email)
            .Matches(RegexTemplates.Email)
            .WithMessage("Invalid email format.");

    }
}
