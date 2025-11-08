using System.Linq.Expressions;

namespace Smart_Freelance_Infrastructure.Common.Pagination.SearchTerms.Configuration;
public class SearchFieldConfiguration<TEntity> : ISearchFieldConfiguration<TEntity>
{
    public Expression<Func<TEntity, string>> FieldSelector { get; }
    public SearchMatchType MatchType { get; }
    public int Priority { get; }
    public double ScoreMultiplier { get; }

    public SearchFieldConfiguration(
        Expression<Func<TEntity, string>> fieldSelector,
        SearchMatchType matchType = SearchMatchType.Contains,
        int priority = 1,
        double scoreMultiplier = 1.0)
    {
        FieldSelector = fieldSelector;
        MatchType = matchType;
        Priority = priority;
        ScoreMultiplier = scoreMultiplier;
    }
}

public class SearchConfiguration<TEntity> : ISearchConfiguration<TEntity> where TEntity : class
{
    public IList<ISearchFieldConfiguration<TEntity>> SearchFields { get; }
    public bool IsSearchEnabled => SearchFields.Any();

    public SearchConfiguration()
    {
        SearchFields = new List<ISearchFieldConfiguration<TEntity>>();
    }

    public SearchConfiguration<TEntity> AddField(
        Expression<Func<TEntity, string>> fieldSelector,
        SearchMatchType matchType = SearchMatchType.Contains,
        int priority = 1,
        double scoreMultiplier = 1.0)
    {
        SearchFields.Add(new SearchFieldConfiguration<TEntity>(fieldSelector, matchType, priority, scoreMultiplier));
        return this;
    }
}
