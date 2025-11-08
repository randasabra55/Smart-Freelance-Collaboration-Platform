using Smart_Freelance_Infrastructure.Common.Pagination.BuilderPattern;

namespace Smart_Freelance_Infrastructure.Common.Pagination.Service;

public class PaginationService : IPaginationService
{


    public IPaginationBuilder<TEntity> For<TEntity>(IQueryable<TEntity> entitiesQuery) where TEntity : class
    {
        return new PaginationBuilder<TEntity>(this, entitiesQuery);
    }
}
