using System;
using System.Linq;
using System.Linq.Expressions;
using DoorManager.Entity.Enum;
using DoorManager.Service.Expressions;

namespace DoorManager.Service.Extensions;

public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var secondBody = expr2.Body.Replace(expr2.Parameters[0], expr1.Parameters[0]);
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, secondBody), expr1.Parameters);
    }

    public static Expression Replace(this Expression expression, Expression searchEx, Expression replaceEx)
    {
        return new ReplaceVisitor(searchEx, replaceEx).Visit(expression);
    }

    public static Type GetRawType(this Expression expression)
    {
        return Nullable.GetUnderlyingType(expression.Type) ?? expression.Type;
    }

    public static Expression GetInExpression(this Expression memberExpression, Type propertyType, string filterValueText)
    {
        var filterValueList = filterValueText.SplitTextToArray()
                                             .Select(x => ExpressionHelper.ConvertToTypeObject(x, propertyType))
                                             .ToList();
        var listOfValuesConst = Expression.Constant(filterValueList, filterValueList.GetType());
        return Expression.Call(
                            listOfValuesConst,
                            listOfValuesConst.Type.GetMethod("Contains", new[] { typeof(object) }),
                            Expression.Convert(memberExpression, typeof(object)));
    }

    public static Expression GetLikeExpression(this Expression memberExpression, string filterValueText)
    {
        var propType = typeof(string);
        if (memberExpression.Type != propType)
        {
            memberExpression = Expression.Call(memberExpression, memberExpression.Type.GetMethod("ToString", Type.EmptyTypes)!);
        }

        return Expression.Call(
                            memberExpression,
                            propType.GetMethod("Contains", new[] { propType }),
                            Expression.Constant(filterValueText, propType));
    }

    public static Expression GetBetweenExpression(this Expression memberExpression, Type propertyType, string filterValueText)
    {
        var filterValueList = filterValueText.SplitTextToArray();

        var minValue = ExpressionHelper.ConvertToTypeObject(filterValueList.First(), propertyType);
        var maxValue = ExpressionHelper.ConvertToTypeObject(filterValueList.Last(), propertyType);

        var minValueConst = Expression.Constant(minValue, memberExpression.Type);
        var maxValueConst = Expression.Constant(maxValue, memberExpression.Type);

        var betweenExpr = Expression.MakeBinary(ExpressionType.GreaterThanOrEqual, memberExpression, minValueConst);
        betweenExpr = Expression.AndAlso(betweenExpr, Expression.MakeBinary(ExpressionType.LessThanOrEqual, memberExpression, maxValueConst));

        return betweenExpr;
    }

    public static Expression GetDefaultExpression(this Expression memberExpression, Type propertyType, string filterValues, FilterOperator filterOperator)
    {
        var filterValueObj = ExpressionHelper.ConvertToTypeObject(filterValues, propertyType);
        var filterValueConst = Expression.Constant(filterValueObj, memberExpression.Type);
        var expressionType = ExpressionHelper.GetExpressionType(filterOperator);

        return Expression.MakeBinary(expressionType, memberExpression, filterValueConst);
    }
}
