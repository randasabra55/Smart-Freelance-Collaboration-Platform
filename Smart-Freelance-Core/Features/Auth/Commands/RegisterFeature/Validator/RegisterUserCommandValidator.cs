using FluentValidation;
using Smart_Freelance_Core.Features.Auth.Commands.RegisterFeature.Handler;
using Smart_Freelance_Core.Features.Auth.Dto;
using Smart_Freelance_Data.Enums;
using Smart_Freelance_Infrastructure.Common.Utility;

namespace Smart_Freelance_Core.Features.Auth.Commands.RegisterFeature.Validator;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(g => g.userRequest)
            .SetValidator(rg => new CreateUserRequestValidator());
    }
}
public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        // Required fields
        RuleFor(g => g.FullName)
            .NotEmpty().WithErrorCode(ErrorCode.InvalidInput.ToString())
            .MaximumLength(100).WithErrorCode(ErrorCode.InvalidInput.ToString());

        RuleFor(g => g.Specialization)
                .NotEmpty().When(g => g.Role == UserType.Freelancer)
                .WithMessage("Specialization is required for freelancers.")
                .MaximumLength(100).WithErrorCode(ErrorCode.InvalidInput.ToString());

        RuleFor(g => g.Email)
            .NotEmpty().WithErrorCode(ErrorCode.InvalidInput.ToString())
            .Matches(RegexTemplates.Email).WithErrorCode(ErrorCode.InvalidEmailFormat.ToString());

        RuleFor(g => g.Password)
            .NotEmpty().WithErrorCode(ErrorCode.InvalidInput.ToString())
            .Matches(RegexTemplates.Password).WithErrorCode(ErrorCode.WeakPassword.ToString());

        RuleFor(g => g.ConfirmPassword)
            .NotEmpty().WithErrorCode(ErrorCode.InvalidInput.ToString())
            .Matches(RegexTemplates.Password).WithErrorCode(ErrorCode.WeakPassword.ToString())
            .Equal(g => g.Password).WithErrorCode(ErrorCode.PasswordMismatch.ToString());

        RuleFor(g => g.Role)
            .NotEmpty()
            .IsInEnum()
            .Must(role => role == UserType.Client || role == UserType.Freelancer)
            .WithMessage("Invalid role. You can only register as a Client or Freelancer.")
            .WithErrorCode(ErrorCode.InvalidInput.ToString());

        RuleFor(g => g.PhoneNumber)
            .NotEmpty().WithErrorCode(ErrorCode.InvalidInput.ToString())
            .MaximumLength(20).WithErrorCode(ErrorCode.InvalidInput.ToString());







    }
}
