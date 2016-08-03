using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using XCode.FluentSql.DbConvension;

namespace XCode.FluentSql.Extensions
{
    public static class ExpressionExtensions
    {
        public static string ToSelectClause(this LambdaExpression lambda, BaseConvension dbConvension)
        {
            var arguments = ((NewExpression)lambda.Body).Arguments;

            var typeList = arguments.Select(a => ((System.Linq.Expressions.MemberExpression)a).Member).ToList();

            return dbConvension.ToSelectClauseString(typeList.Select(a =>  Tuple.Create<string, string>(a.ReflectedType.Name, a.Name)));
        }

        public static string BuildWhereClauseString(this Expression expression, BaseConvension dbConvention)
        {

            var builder = PredicateParser(expression, dbConvention);
            return dbConvention.ToWhereClauseString(builder);
        }

        public static IList<string> PredicateParser(this Expression expression, BaseConvension dbConvention, bool getValueFromRhf = true)
        {
            var expressionParser = new ExpressionParser(dbConvention, getValueFromRhf);
            return expressionParser.ParsePredicate(expression);
        } 
    }
}
