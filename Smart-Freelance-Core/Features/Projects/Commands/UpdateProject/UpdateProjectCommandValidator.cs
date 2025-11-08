using FluentValidation;
using Smart_Freelance_Data.Enums;

namespace Smart_Freelance_Core.Features.Projects.Commands.UpdateProject
{
    public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
    {
        public UpdateProjectCommandValidator()
        {
            RuleFor(x => x.ProjectId)
               .GreaterThan(0)
               .WithErrorCode(ErrorCode.InvalidInput.ToString());

            RuleFor(x => x.Title)
                 .NotEmpty().WithMessage("Title is required.")
                 .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.Budget)
                .GreaterThan(0).WithMessage("Budget must be greater than zero.");

            RuleFor(x => x.Deadline)
                .GreaterThan(DateTime.Now).WithMessage("Deadline must be a future date.");
        }
    }
}
