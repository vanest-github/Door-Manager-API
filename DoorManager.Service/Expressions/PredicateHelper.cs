using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DoorManager.Entity.DTO;
using DoorManager.Entity.Enum;
using DoorManager.Service.Extensions;

namespace DoorManager.Service.Expressions;

public static class PredicateHelper
{
    public static Expression<Func<T, bool>> CreateFilterPredicate<T>(IEnumerable<FilterDto> filterParameters)
    {
        var predicate = ExpressionHelper.True<T>();
        if (filterParameters != null && filterParameters.Any())
        {
            var paramExpression = Expression.Parameter(typeof(T), "x");
            foreach (var filter in filterParameters)
            {
                var left = Expression.Property(paramExpression, filter.ColumnName);
                var expression = GetFilterExpression(filter, left);
                var lambdaExpression = Expression.Lambda<Func<T, bool>>(expression, paramExpression);
                predicate = predicate.And(lambdaExpression);
            }
        }

        return predicate;
    }

    private static Expression GetFilterExpression(FilterDto filter, Expression paramExpression)
    {
        var propType = paramExpression.GetRawType();
        var filterExpression = filter.Operator switch
        {
            FilterOperator.Like => paramExpression.GetLikeExpression(filter.Values),
            FilterOperator.In => paramExpression.GetInExpression(propType, filter.Values),
            FilterOperator.Between => paramExpression.GetBetweenExpression(propType, filter.Values),
            _ => paramExpression.GetDefaultExpression(propType, filter.Values, filter.Operator),
        };

        return filterExpression;
    }
}
