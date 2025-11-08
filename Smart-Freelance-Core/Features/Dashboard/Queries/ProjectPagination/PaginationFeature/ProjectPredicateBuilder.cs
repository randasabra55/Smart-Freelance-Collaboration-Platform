using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Data.Entities;
using Smart_Freelance_Infrastructure.Common.Utility;
using System.Linq.Expressions;

namespace Smart_Freelance_Core.Features.Dashboard.Queries.ProjectPagination.PaginationFeature
{
    public class ProjectPredicateBuilder
    {
        public static Expression<Func<Project, bool>> BuildPredicate(ProjectPaginationModel model, Expression<Func<Project, bool>> predicate)
        {
            if (model.AssignedFreelancerId.HasValue)
            {
                predicate = predicate.And(c => c.AssignedFreelancerId == model.AssignedFreelancerId);
            }

            if (model.ClientId.HasValue)
            {
                predicate = predicate.And(c => c.ClientId == model.ClientId);
            }

            if (model.Status.HasValue)
            {
                predicate = predicate.And(c => c.Status == model.Status);
            }

            if (model.Id.HasValue)
            {
                predicate = predicate.And(c => c.Id == model.Id);
            }

            if (!string.IsNullOrEmpty(model.Title))
            {
                predicate = predicate.And(p => EF.Functions.Like(p.Title, model.Title + "%"));
            }

            if (!string.IsNullOrEmpty(model.Description))
            {
                predicate = predicate.And(p => EF.Functions.Like(p.Description, model.Description + "%"));
            }

            if (!string.IsNullOrEmpty(model.CategoryName))
            {
                predicate = predicate.And(p => EF.Functions.Like(p.CategoryName, model.CategoryName + "%"));
            }

            if (model.Budget.HasValue)
            {
                predicate = predicate.And(c => c.Budget == model.Budget);
            }

            if (model.Deadline.HasValue)
            {
                predicate = predicate.And(c => c.Deadline == model.Deadline);
            }

            if (model.CreatedAt.HasValue)
            {
                predicate = predicate.And(c => c.CreatedAt == model.CreatedAt);
            }


            return predicate;
        }
    }
}
