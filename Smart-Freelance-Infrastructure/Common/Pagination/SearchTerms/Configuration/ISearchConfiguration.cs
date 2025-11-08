using System.Linq.Expressions;

namespace Smart_Freelance_Infrastructure.Common.Pagination.SearchTerms.Configuration;

public interface ISearchFieldConfiguration<TEntity>
{
    Expression<Func<TEntity, string>> FieldSelector { get; }
    SearchMatchType MatchType { get; }
    int Priority { get; }
    double ScoreMultiplier { get; }
}

public interface ISearchConfiguration<TEntity> where TEntity : class
{
    IList<ISearchFieldConfiguration<TEntity>> SearchFields { get; }
    bool IsSearchEnabled { get; }
}
