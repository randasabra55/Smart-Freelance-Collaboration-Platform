using Smart_Freelance_Infrastructure.Common.Pagination.SearchTerms.Configuration;
using Smart_Freelance_Infrastructure.Common.Utility;
using System.Linq.Expressions;
//using Smart_Freelance_Infrastructure.Common.Utility;

namespace Smart_Freelance_Infrastructure.Common.Pagination.SearchTerms;
public static class SearchExpressionBuilder
{
    public static Expression<Func<TEntity, bool>> BuildSearchPredicate<TEntity>(
        ISearchConfiguration<TEntity> searchConfig,
        string searchTerm) where TEntity : class
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || !searchConfig.IsSearchEnabled)
        {
            return PredicateBuilder.True<TEntity>();
        }

        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        Expression? combinedExpression = null;

        foreach (var field in searchConfig.SearchFields)
        {
            var fieldExpression = CreateFieldSearchExpression(parameter, field, searchTerm);
            combinedExpression = combinedExpression == null
                ? fieldExpression
                : Expression.OrElse(combinedExpression, fieldExpression);
        }

        return combinedExpression != null
            ? Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter)
            : PredicateBuilder.True<TEntity>();
    }

    private static Expression CreateFieldSearchExpression<TEntity>(
        ParameterExpression parameter,
        ISearchFieldConfiguration<TEntity> field,
        string searchTerm)
    {
        // Replace the parameter in the field selector
        var fieldAccess = new ParameterReplacer(parameter).Visit(field.FieldSelector.Body);

        // Null check
        var nullCheck = Expression.NotEqual(fieldAccess, Expression.Constant(null));

        // Create the search condition based on match type
        var searchCondition = field.MatchType switch
        {
            SearchMatchType.Contains => CreateContainsExpression(fieldAccess, searchTerm),
            SearchMatchType.StartsWith => CreateStartsWithExpression(fieldAccess, searchTerm),
            SearchMatchType.EndsWith => CreateEndsWithExpression(fieldAccess, searchTerm),
            SearchMatchType.Exact => CreateExactExpression(fieldAccess, searchTerm),
            SearchMatchType.FullTextSearch => CreateFullTextSearchExpression(fieldAccess, searchTerm),
            _ => CreateContainsExpression(fieldAccess, searchTerm)
        };

        return Expression.AndAlso(nullCheck, searchCondition);
    }

    private static Expression CreateContainsExpression(Expression fieldAccess, string searchTerm)
    {
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

        var lowerField = Expression.Call(fieldAccess, toLowerMethod!);
        var lowerSearchTerm = Expression.Constant(searchTerm.ToLower());

        return Expression.Call(lowerField, containsMethod!, lowerSearchTerm);
    }

    private static Expression CreateStartsWithExpression(Expression fieldAccess, string searchTerm)
    {
        var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

        var lowerField = Expression.Call(fieldAccess, toLowerMethod!);
        var lowerSearchTerm = Expression.Constant(searchTerm.ToLower());

        return Expression.Call(lowerField, startsWithMethod!, lowerSearchTerm);
    }

    private static Expression CreateEndsWithExpression(Expression fieldAccess, string searchTerm)
    {
        var endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

        var lowerField = Expression.Call(fieldAccess, toLowerMethod!);
        var lowerSearchTerm = Expression.Constant(searchTerm.ToLower());

        return Expression.Call(lowerField, endsWithMethod!, lowerSearchTerm);
    }

    private static Expression CreateExactExpression(Expression fieldAccess, string searchTerm)
    {
        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
        var fieldToLower = Expression.Call(fieldAccess, toLowerMethod!);

        var searchTermLower = Expression.Constant(searchTerm.ToLower());

        return Expression.Equal(fieldToLower, searchTermLower);
    }

    private static Expression CreateFullTextSearchExpression(Expression fieldAccess, string searchTerm)
    {

        return CreateContainsExpression(fieldAccess, searchTerm);
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        public ParameterReplacer(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _parameter;
        }
    }
}
