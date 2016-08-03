using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XCode.FluentSql.DbConvension;

namespace XCode.FluentSql.Extensions
{
    public static class MethodCallExtensions
    {
        public static string Parse(this MethodCallExpression methodCallExpression, BaseConvension convention)
        {
            if (methodCallExpression == null) throw new ArgumentNullException("Not a valid method call Expression.");
            switch (methodCallExpression.Method.Name)
            {
                case "Contains":
                    return methodCallExpression.ParseContains(convention);
                case "Equals":
                case "StartsWith":
                case "EndsWith":
                default: throw new InvalidOperationException();
            }
        }

        private static string ParseContains(this MethodCallExpression methodCallExpression, BaseConvension convension)
        {
            if (methodCallExpression.Arguments.Count != 1) throw new NotSupportedException();
            var format = "{0} IN ({1})"; //TODO:: move this format to dbConvesion
            

            var declaringType = methodCallExpression.Method.DeclaringType;
            if (!declaringType.IsGenericType || declaringType.GetGenericTypeDefinition() != typeof(List<>)) throw new NotSupportedException();
            var argument = methodCallExpression.Arguments.First() as MemberExpression;
            var caller = methodCallExpression.Object as MemberExpression;
            var items = caller.GetValue<List<string>>().ToCommaSeparatedList(convension);

            return convension.ToContainsString(argument.GetTypeName(convension), items);
        }
    }

    public static class MemberExpressionExtensions
    {
        public static T GetValue<T>(this MemberExpression memberExpression)
        {
            return Expression.Lambda<Func<T>>(Expression.Convert(memberExpression, typeof(T))).Compile()();
        }

        public static string GetTypeName(this MemberExpression memberExpression, BaseConvension convension)
        {
            var tuple = Tuple.Create<string, string>(memberExpression.Member.Name, memberExpression.Member.DeclaringType.Name);
            return convension.ToQuantifiedNameConvention(memberExpression.Member.DeclaringType.Name, memberExpression.Member.Name);
        }

        public static string ToCommaSeparatedList(this List<string> source, BaseConvension convension)
        {
            var commaSeparated = new StringBuilder();
            var first = true;
            foreach (var item in source)
            {
                if (first) { first = false; }
                else commaSeparated.Append(", ");
                commaSeparated.Append(convension.ToValueNameConvention(item));
            }
            return commaSeparated.ToString();
        }
    }
}
