
using FluentValidation;
using Smart_Freelance_Infrastructure.Common.Utility;

namespace Smart_Freelance_Core.Passwords.Commands.VerifyOTP
{
    public class VerifyOTPValidator : AbstractValidator<verifyOTPCommand>
    {
        public VerifyOTPValidator()
        {
            RuleFor(x => x.Email)
            .Matches(RegexTemplates.Email)
            .WithMessage("Invalid email format.");


            RuleFor(x => x.OTP)
                .NotEmpty()
                .WithMessage("OTP is required.")
                .Length(6)
                .WithMessage("OTP must be 6 digits long.");
        }
    }
}
