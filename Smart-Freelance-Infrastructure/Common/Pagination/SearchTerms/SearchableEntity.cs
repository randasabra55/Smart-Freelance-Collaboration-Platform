namespace Smart_Freelance_Infrastructure.Common.Pagination.SearchTerms;
public class SearchableEntity<TEntity, TResult>
{
    public TEntity Entity { get; set; }
    public TResult ProjectedResult { get; set; }
    public double SearchScore { get; set; }
    public int HighestPriorityMatch { get; set; }

    public SearchableEntity(TEntity entity, TResult projectedResult, double searchScore, int highestPriorityMatch)
    {
        Entity = entity;
        ProjectedResult = projectedResult;
        SearchScore = searchScore;
        HighestPriorityMatch = highestPriorityMatch;
    }
}
