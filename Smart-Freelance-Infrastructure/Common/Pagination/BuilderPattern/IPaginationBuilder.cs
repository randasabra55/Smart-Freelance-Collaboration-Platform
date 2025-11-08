using Smart_Freelance_Infrastructure.Common.Pagination.SearchTerms.Configuration;
using Smart_Freelance_Infrastructure.Common.Responses;
using System.Linq.Expressions;

namespace Smart_Freelance_Infrastructure.Common.Pagination.BuilderPattern;
public interface IPaginationBuilder<TEntity> where TEntity : class
{
    IPaginationBuilderWithModel<TEntity, TModel> WithModel<TModel>(TModel model) where TModel : IBasePaginationModel;
}

public interface IPaginationBuilderWithModel<TEntity, TModel>
    where TEntity : class
    where TModel : IBasePaginationModel
{
    IPaginationBuilderWithSearch<TEntity, TModel> WithSearch(ISearchConfiguration<TEntity> searchConfig);

    IPaginationBuilderWithPredicate<TEntity, TModel> WithPredicate(
        Func<TModel, Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>> buildPredicate);

    IPaginationBuilderWithOrdering<TEntity, TModel> WithOrdering(
        Func<TModel, IQueryable<TEntity>, IQueryable<TEntity>> applyOrdering);
}

public interface IPaginationBuilderWithSearch<TEntity, TModel>
    where TEntity : class
    where TModel : IBasePaginationModel
{
    IPaginationBuilderWithPredicate<TEntity, TModel> WithPredicate(
        Func<TModel, Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>> buildPredicate);

    IPaginationBuilderWithOrdering<TEntity, TModel> WithOrdering(
        Func<TModel, IQueryable<TEntity>, IQueryable<TEntity>> applyOrdering);
}

public interface IPaginationBuilderWithPredicate<TEntity, TModel>
    where TEntity : class
    where TModel : IBasePaginationModel
{
    IPaginationBuilderWithOrdering<TEntity, TModel> WithOrdering(
        Func<TModel, IQueryable<TEntity>, IQueryable<TEntity>> applyOrdering);
}

public interface IPaginationBuilderWithOrdering<TEntity, TModel>
    where TEntity : class
    where TModel : IBasePaginationModel
{
    IPaginationBuilderReady<TEntity, TModel> IgnoreQueryFilters(bool ignore = true);

    Task<PaginatedList<TResult>> SelectAsync<TResult>(Expression<Func<TEntity, TResult>> projection)
        where TResult : class;
}

public interface IPaginationBuilderReady<TEntity, TModel>
    where TEntity : class
    where TModel : IBasePaginationModel
{
    Task<PaginatedList<TResult>> SelectAsync<TResult>(Expression<Func<TEntity, TResult>> projection)
        where TResult : class;
}
