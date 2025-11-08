
using Smart_Freelance_Infrastructure.Common.Pagination.BuilderPattern;

namespace Smart_Freelance_Infrastructure.Common.Pagination.Service;
public interface IPaginationService
{

    IPaginationBuilder<TEntity> For<TEntity>(IQueryable<TEntity> entitiesQuery) where TEntity : class;


}
