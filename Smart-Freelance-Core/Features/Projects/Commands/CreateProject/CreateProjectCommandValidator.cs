using FluentValidation;

namespace Smart_Freelance_Core.Features.Projects.Commands.CreateProject
{
    public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
    {
        public CreateProjectCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Category is required.")
                .MaximumLength(1000).WithMessage("Category must not exceed 1000 characters.");

            RuleFor(x => x.Budget)
                .GreaterThan(0).WithMessage("Budget must be greater than zero.");

            RuleFor(x => x.Deadline)
                .GreaterThan(DateTime.Now).WithMessage("Deadline must be a future date.");

        }
    }
}
