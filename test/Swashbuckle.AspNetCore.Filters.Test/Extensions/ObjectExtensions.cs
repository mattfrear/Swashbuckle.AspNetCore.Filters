using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Swashbuckle.AspNetCore.Filters.Test.Extensions
{
    public static class ObjectExtensions
    {
        public static T With<T, TProp>(this T o, Expression<Func<T, TProp>> prop, TProp value)
        {
            var memberExpression = prop.Body as MemberExpression;

            if (memberExpression == null)
                throw new Exception($"A property must be provided. ({prop})");

            var propertyInfo = (PropertyInfo) memberExpression.Member;

            if (propertyInfo.CanWrite)
                propertyInfo.SetValue(o, value);
            else
                typeof(T).GetField($"<{propertyInfo.Name}>k__BackingField",
                    BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(o, value);

            return o;
        }
    }
}