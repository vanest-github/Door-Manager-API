using System;
using System.ComponentModel;
using System.Linq.Expressions;
using DoorManager.Entity.Enum;

namespace DoorManager.Service.Expressions;

public static class ExpressionHelper
{
    public static Expression<Func<T, bool>> True<T>()
    {
        return x => true;
    }

    public static object ConvertToTypeObject(string value, Type type)
    {
        if (type == typeof(string))
        {
            return value;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        try
        {
            var converter = TypeDescriptor.GetConverter(type);
            return converter.ConvertFrom(value.Trim());
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static ExpressionType GetExpressionType(FilterOperator filterOperator)
    {
        _ = Enum.TryParse(filterOperator.ToString(), out ExpressionType expressionType);
        return expressionType;
    }
}
