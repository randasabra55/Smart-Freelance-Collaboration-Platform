using Microsoft.EntityFrameworkCore;
using Smart_Freelance_Infrastructure.Common.Pagination.SearchTerms;
using Smart_Freelance_Infrastructure.Common.Pagination.SearchTerms.Configuration;
using Smart_Freelance_Infrastructure.Common.Pagination.Service;
using Smart_Freelance_Infrastructure.Common.Responses;
using Smart_Freelance_Infrastructure.Common.Utility;
using System.Linq.Expressions;

namespace Smart_Freelance_Infrastructure.Common.Pagination.BuilderPattern;

internal class PaginationBuilderWithModel<TEntity, TModel> : IPaginationBuilderWithModel<TEntity, TModel>
    where TEntity : class
    where TModel : IBasePaginationModel
{
    private readonly IPaginationService _service;
    private readonly IQueryable<TEntity> _entitiesQuery;
    private readonly TModel _model;

    public PaginationBuilderWithModel(IPaginationService service, IQueryable<TEntity> entitiesQuery, TModel model)
    {
        _service = service;
        _entitiesQuery = entitiesQuery;
        _model = model;
    }

    public IPaginationBuilderWithSearch<TEntity, TModel> WithSearch(ISearchConfiguration<TEntity> searchConfig)
    {
        return new PaginationBuilderWithSearch<TEntity, TModel>(_service, _entitiesQuery, _model, searchConfig);
    }

    public IPaginationBuilderWithPredicate<TEntity, TModel> WithPredicate(
        Func<TModel, Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>> buildPredicate)
    {
        var emptySearchConfig = new SearchConfiguration<TEntity>();
        return new PaginationBuilderWithPredicate<TEntity, TModel>(_service, _entitiesQuery, _model, emptySearchConfig, buildPredicate);
    }

    public IPaginationBuilderWithOrdering<TEntity, TModel> WithOrdering(
        Func<TModel, IQueryable<TEntity>, IQueryable<TEntity>> applyOrdering)
    {
        var emptySearchConfig = new SearchConfiguration<TEntity>();
        var defaultPredicate = (TModel m, Expression<Func<TEntity, bool>> p) => p;
        return new PaginationBuilderWithOrdering<TEntity, TModel>(_service, _entitiesQuery, _model, emptySearchConfig, defaultPredicate, applyOrdering);
    }
}

internal class PaginationBuilderWithSearch<TEntity, TModel> : IPaginationBuilderWithSearch<TEntity, TModel>
    where TEntity : class
    where TModel : IBasePaginationModel
{
    private readonly IPaginationService _service;
    private readonly IQueryable<TEntity> _entitiesQuery;
    private readonly TModel _model;
    private readonly ISearchConfiguration<TEntity> _searchConfig;

    public PaginationBuilderWithSearch(IPaginationService service, IQueryable<TEntity> entitiesQuery, TModel model, ISearchConfiguration<TEntity> searchConfig)
    {
        _service = service;
        _entitiesQuery = entitiesQuery;
        _model = model;
        _searchConfig = searchConfig;
    }

    public IPaginationBuilderWithPredicate<TEntity, TModel> WithPredicate(
        Func<TModel, Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>> buildPredicate)
    {
        return new PaginationBuilderWithPredicate<TEntity, TModel>(_service, _entitiesQuery, _model, _searchConfig, buildPredicate);
    }

    public IPaginationBuilderWithOrdering<TEntity, TModel> WithOrdering(
        Func<TModel, IQueryable<TEntity>, IQueryable<TEntity>> applyOrdering)
    {
        var defaultPredicate = (TModel m, Expression<Func<TEntity, bool>> p) => p;
        return new PaginationBuilderWithOrdering<TEntity, TModel>(_service, _entitiesQuery, _model, _searchConfig, defaultPredicate, applyOrdering);
    }
}

internal class PaginationBuilderWithPredicate<TEntity, TModel> : IPaginationBuilderWithPredicate<TEntity, TModel>
    where TEntity : class
    where TModel : IBasePaginationModel
{
    private readonly IPaginationService _service;
    private readonly IQueryable<TEntity> _entitiesQuery;
    private readonly TModel _model;
    private readonly ISearchConfiguration<TEntity> _searchConfig;
    private readonly Func<TModel, Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>> _buildPredicate;

    public PaginationBuilderWithPredicate(
        IPaginationService service,
        IQueryable<TEntity> entitiesQuery,
        TModel model,
        ISearchConfiguration<TEntity> searchConfig,
        Func<TModel, Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>> buildPredicate)
    {
        _service = service;
        _entitiesQuery = entitiesQuery;
        _model = model;
        _searchConfig = searchConfig;
        _buildPredicate = buildPredicate;
    }

    public IPaginationBuilderWithOrdering<TEntity, TModel> WithOrdering(
        Func<TModel, IQueryable<TEntity>, IQueryable<TEntity>> applyOrdering)
    {
        return new PaginationBuilderWithOrdering<TEntity, TModel>(_service, _entitiesQuery, _model, _searchConfig, _buildPredicate, applyOrdering);
    }
}

internal class PaginationBuilder<TEntity> : IPaginationBuilder<TEntity>
    where TEntity : class
{
    private readonly IPaginationService _service;
    private readonly IQueryable<TEntity> _entitiesQuery;

    public PaginationBuilder(IPaginationService service, IQueryable<TEntity> entitiesQuery)
    {
        _service = service;
        _entitiesQuery = entitiesQuery;
    }

    public IPaginationBuilderWithModel<TEntity, TModel> WithModel<TModel>(TModel model)
        where TModel : IBasePaginationModel
    {
        return new PaginationBuilderWithModel<TEntity, TModel>(_service, _entitiesQuery, model);
    }
}

internal class PaginationBuilderWithOrdering<TEntity, TModel> : IPaginationBuilderWithOrdering<TEntity, TModel>
    where TEntity : class
    where TModel : IBasePaginationModel
{
    private readonly IPaginationService _service;
    private readonly IQueryable<TEntity> _entitiesQuery;
    private readonly TModel _model;
    private readonly ISearchConfiguration<TEntity> _searchConfig;
    private readonly Func<TModel, Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>> _buildPredicate;
    private readonly Func<TModel, IQueryable<TEntity>, IQueryable<TEntity>> _applyOrdering;

    public PaginationBuilderWithOrdering(
        IPaginationService service,
        IQueryable<TEntity> entitiesQuery,
        TModel model,
        ISearchConfiguration<TEntity> searchConfig,
        Func<TModel, Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>> buildPredicate,
        Func<TModel, IQueryable<TEntity>, IQueryable<TEntity>> applyOrdering)
    {
        _service = service;
        _entitiesQuery = entitiesQuery;
        _model = model;
        _searchConfig = searchConfig;
        _buildPredicate = buildPredicate;
        _applyOrdering = applyOrdering;
    }

    public IPaginationBuilderReady<TEntity, TModel> IgnoreQueryFilters(bool ignore = true)
    {
        return new PaginationBuilderReady<TEntity, TModel>(_service, _entitiesQuery, _model, _searchConfig, _buildPredicate, _applyOrdering, ignore);
    }

    public async Task<PaginatedList<TResult>> SelectAsync<TResult>(Expression<Func<TEntity, TResult>> projection)
        where TResult : class
    {
        return await ExecutePaginationAsync(projection, false);
    }

    private async Task<PaginatedList<TResult>> ExecutePaginationAsync<TResult>(
        Expression<Func<TEntity, TResult>> projection,
        bool ignoreQueryFilters) where TResult : class
    {
        var query = _entitiesQuery.AsQueryable();

        if (ignoreQueryFilters)
            query = query.IgnoreQueryFilters();

        var predicate = PredicateBuilder.True<TEntity>();
        var entityPredicate = _buildPredicate(_model, predicate);
        if (entityPredicate != null)
            query = query.Where(entityPredicate);

        if (!string.IsNullOrWhiteSpace(_model.SearchTerm) && _searchConfig.IsSearchEnabled)
        {
            return await ExecuteSearchPaginationAsync(query, projection);
        }

        query = _applyOrdering(_model, query);
        int count = await query.CountAsync();

        if (_model.PaginationOff)
            return await GetAllResultsAsync(query, count, projection);

        return await GetPaginatedResultsAsync(query, count, projection);
    }

    private async Task<PaginatedList<TResult>> ExecuteSearchPaginationAsync<TResult>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, TResult>> projection) where TResult : class
    {
        var searchPredicate = SearchExpressionBuilder.BuildSearchPredicate(_searchConfig, _model.SearchTerm!);
        query = query.Where(searchPredicate);

        // Apply ordering at the database level
        query = _applyOrdering(_model, query);

        int totalCount = await query.CountAsync();

        if (_model.PaginationOff)
        {
            return await GetAllResultsAsync(query, totalCount, projection);
        }

        return await GetPaginatedResultsAsync(query, totalCount, projection);
    }

    private async Task<PaginatedList<TResult>> GetAllResultsAsync<TResult>(
        IQueryable<TEntity> query,
        int count,
        Expression<Func<TEntity, TResult>> projection) where TResult : class
    {
        List<TResult> items = await query.Select(projection).ToListAsync();
        return new PaginatedList<TResult>(items, count, 1, count);
    }

    private async Task<PaginatedList<TResult>> GetPaginatedResultsAsync<TResult>(
        IQueryable<TEntity> query,
        int count,
        Expression<Func<TEntity, TResult>> projection) where TResult : class
    {
        var paginatedQuery = query
            .Skip((_model.PageIndex - 1) * _model.PageSize)
            .Take(_model.PageSize);

        List<TResult> items = await paginatedQuery.Select(projection).ToListAsync();
        return new PaginatedList<TResult>(items, count, _model.PageIndex, _model.PageSize);
    }

    private SearchableEntity<TEntity, TResult> CalculateSearchScore<TResult>(TEntity entity, string searchTerm, TResult projectedResult)
    {
        double totalScore = 0;
        int highestPriorityMatch = int.MaxValue;

        foreach (var field in _searchConfig.SearchFields)
        {
            var fieldValue = field.FieldSelector.Compile()(entity);
            if (string.IsNullOrEmpty(fieldValue)) continue;

            double fieldScore = CalculateFieldScore(fieldValue, searchTerm, field.MatchType);
            if (fieldScore > 0)
            {
                totalScore += fieldScore * field.ScoreMultiplier;
                if (field.Priority < highestPriorityMatch)
                {
                    highestPriorityMatch = field.Priority;
                }
            }
        }

        return new SearchableEntity<TEntity, TResult>(entity, projectedResult, totalScore, highestPriorityMatch);
    }

    private static double CalculateFieldScore(string fieldValue, string searchTerm, SearchMatchType matchType)
    {
        var lowerFieldValue = fieldValue.ToLower();
        var lowerSearchTerm = searchTerm.ToLower();

        return matchType switch
        {
            SearchMatchType.Exact when lowerFieldValue == lowerSearchTerm => 100.0,
            SearchMatchType.StartsWith when lowerFieldValue.StartsWith(lowerSearchTerm) => 80.0,
            SearchMatchType.EndsWith when lowerFieldValue.EndsWith(lowerSearchTerm) => 60.0,
            SearchMatchType.Contains when lowerFieldValue.Contains(lowerSearchTerm) => 40.0,
            SearchMatchType.FullTextSearch when lowerFieldValue.Contains(lowerSearchTerm) => 30.0,
            _ => 0.0
        };
    }
}

internal class PaginationBuilderReady<TEntity, TModel> : IPaginationBuilderReady<TEntity, TModel>
    where TEntity : class
    where TModel : IBasePaginationModel
{
    private readonly IPaginationService _service;
    private readonly IQueryable<TEntity> _entitiesQuery;
    private readonly TModel _model;
    private readonly ISearchConfiguration<TEntity> _searchConfig;
    private readonly Func<TModel, Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>> _buildPredicate;
    private readonly Func<TModel, IQueryable<TEntity>, IQueryable<TEntity>> _applyOrdering;
    private readonly bool _ignoreQueryFilters;

    public PaginationBuilderReady(
        IPaginationService service,
        IQueryable<TEntity> entitiesQuery,
        TModel model,
        ISearchConfiguration<TEntity> searchConfig,
        Func<TModel, Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>> buildPredicate,
        Func<TModel, IQueryable<TEntity>, IQueryable<TEntity>> applyOrdering,
        bool ignoreQueryFilters)
    {
        _service = service;
        _entitiesQuery = entitiesQuery;
        _model = model;
        _searchConfig = searchConfig;
        _buildPredicate = buildPredicate;
        _applyOrdering = applyOrdering;
        _ignoreQueryFilters = ignoreQueryFilters;
    }

    public async Task<PaginatedList<TResult>> SelectAsync<TResult>(Expression<Func<TEntity, TResult>> projection)
        where TResult : class
    {
        return await ExecutePaginationAsync(projection);
    }

    private async Task<PaginatedList<TResult>> ExecutePaginationAsync<TResult>(
        Expression<Func<TEntity, TResult>> projection) where TResult : class
    {
        var query = _entitiesQuery.AsQueryable();

        if (_ignoreQueryFilters)
            query = query.IgnoreQueryFilters();

        var predicate = PredicateBuilder.True<TEntity>();
        var entityPredicate = _buildPredicate(_model, predicate);
        if (entityPredicate != null)
            query = query.Where(entityPredicate);

        if (!string.IsNullOrWhiteSpace(_model.SearchTerm) && _searchConfig.IsSearchEnabled)
        {
            return await ExecuteSearchPaginationAsync(query, projection);
        }

        query = _applyOrdering(_model, query);
        int count = await query.CountAsync();

        if (_model.PaginationOff)
            return await GetAllResultsAsync(query, count, projection);

        return await GetPaginatedResultsAsync(query, count, projection);
    }

    private async Task<PaginatedList<TResult>> ExecuteSearchPaginationAsync<TResult>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, TResult>> projection) where TResult : class
    {
        var searchPredicate = SearchExpressionBuilder.BuildSearchPredicate(_searchConfig, _model.SearchTerm!);
        query = query.Where(searchPredicate);

        query = _applyOrdering(_model, query);

        int totalCount = await query.CountAsync();

        if (_model.PaginationOff)
        {
            return await GetAllResultsAsync(query, totalCount, projection);
        }

        return await GetPaginatedResultsAsync(query, totalCount, projection);
    }

    private SearchableEntity<TEntity, TResult> CalculateSearchScore<TResult>(TEntity entity, string searchTerm, TResult projectedResult)
    {
        double totalScore = 0;
        int highestPriorityMatch = int.MaxValue;

        foreach (var field in _searchConfig.SearchFields)
        {
            var fieldValue = field.FieldSelector.Compile()(entity);
            if (string.IsNullOrEmpty(fieldValue)) continue;

            double fieldScore = CalculateFieldScore(fieldValue, searchTerm, field.MatchType);
            if (fieldScore > 0)
            {
                totalScore += fieldScore * field.ScoreMultiplier;
                if (field.Priority < highestPriorityMatch)
                {
                    highestPriorityMatch = field.Priority;
                }
            }
        }

        return new SearchableEntity<TEntity, TResult>(entity, projectedResult, totalScore, highestPriorityMatch);
    }

    private static double CalculateFieldScore(string fieldValue, string searchTerm, SearchMatchType matchType)
    {
        var lowerFieldValue = fieldValue.ToLower();
        var lowerSearchTerm = searchTerm.ToLower();

        return matchType switch
        {
            SearchMatchType.Exact when lowerFieldValue == lowerSearchTerm => 100.0,
            SearchMatchType.StartsWith when lowerFieldValue.StartsWith(lowerSearchTerm) => 80.0,
            SearchMatchType.EndsWith when lowerFieldValue.EndsWith(lowerSearchTerm) => 60.0,
            SearchMatchType.Contains when lowerFieldValue.Contains(lowerSearchTerm) => 40.0,
            SearchMatchType.FullTextSearch when lowerFieldValue.Contains(lowerSearchTerm) => 30.0,
            _ => 0.0
        };
    }

    private async Task<PaginatedList<TResult>> GetAllResultsAsync<TResult>(
        IQueryable<TEntity> query,
        int count,
        Expression<Func<TEntity, TResult>> projection) where TResult : class
    {
        List<TResult> items = await query.Select(projection).ToListAsync();
        return new PaginatedList<TResult>(items, count, 1, count);
    }

    private async Task<PaginatedList<TResult>> GetPaginatedResultsAsync<TResult>(
        IQueryable<TEntity> query,
        int count,
        Expression<Func<TEntity, TResult>> projection) where TResult : class
    {
        var paginatedQuery = query
            .Skip((_model.PageIndex - 1) * _model.PageSize)
            .Take(_model.PageSize);

        List<TResult> items = await paginatedQuery.Select(projection).ToListAsync();
        return new PaginatedList<TResult>(items, count, _model.PageIndex, _model.PageSize);
    }
}
