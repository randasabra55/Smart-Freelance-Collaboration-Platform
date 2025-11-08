using FluentValidation;
using Smart_Freelance_Data.Enums;

namespace Smart_Freelance_Core.Features.Projects.Queries.GetProjectById
{
    public class GetProjectByIdQueryValidator : AbstractValidator<GetProjectByIdQuery>
    {
        public GetProjectByIdQueryValidator()
        {
            RuleFor(x => x.Id)
              .GreaterThan(0)
              .WithErrorCode(ErrorCode.InvalidInput.ToString());
        }
    }
}
