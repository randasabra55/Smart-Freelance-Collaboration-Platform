using FluentValidation;
using Smart_Freelance_Data.Enums;

namespace Smart_Freelance_Core.Features.Projects.Commands.DeleteProject
{
    public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
    {
        public DeleteProjectCommandValidator()
        {
            RuleFor(x => x.ProjectId)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.InvalidInput.ToString());
        }
    }
}
