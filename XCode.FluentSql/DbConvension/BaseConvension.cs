using System;
using System.Collections.Generic;
using System.Linq;
using XCode.FluentSql.Enums;
using XCode.FluentSql.Extensions;

namespace XCode.FluentSql.DbConvension
{
    public abstract class BaseConvension
    {

        protected BaseConvension()
        {
            ToTableNameConvention = name => EntityWrapper + name.Pluralize() + EntityWrapper;
            ToEntityNameConvention = name => EntityWrapper + name + EntityWrapper;
            ToProjectionConvetion = (a, b) => string.Format("{0}{1}{2}", a, EntitySeparator, b);
            ToValueNameConvention = name => string.Format("{0}{1}{0}", ValueWrapper, name);
            ToQuantifiedNameConvention = (typeName, propertyName) => ToTableNameConvention(typeName) + "." + ToEntityNameConvention(propertyName);
            ToClauseConvention = (a, b) => a + ClauseSeparator + b;
            SelectClause = "SELECT";
            WhereClause = "WHERE";
            JoinClause = "JOIN";
            JoinConditionClause = "ON";
            FromClause = "FROM";
            OrderByClause = "ORDER BY";
            GroupByClause = "GROUP BY";
            ValueWrapper = "`";
            EntityWrapper = string.Empty;
            EntitySeparator = ", ";

            ClauseSeparator = " ";

        }
        public static string SelectClause { get; protected set; }
        public static string WhereClause { get; protected set; }
        public static string JoinClause { get; protected set; }
        public static string JoinConditionClause { get; protected set; }
        public static string FromClause { get; protected set; }
        public static string OrderByClause { get; protected set; }
        public static string GroupByClause { get; protected set; }
        public static string ValueWrapper { get; protected set; }

        public static string EntityWrapper { get; protected set; }
        public static string EntitySeparator { get; protected set; }
        public static string ClauseSeparator { get; protected set; }

        public readonly Func<string, string> ToValueNameConvention;

        public readonly Func<string, string> ToEntityNameConvention;
        public readonly Func<string, string> ToTableNameConvention;


        public readonly Func<string, string, string> ToQuantifiedNameConvention;

        public readonly Func<string, string, string> ToProjectionConvetion;

        public readonly Func<string, string, string> ToClauseConvention;

        public string ToJoinClauseConvention(string entity, IList<string> conditionList, JoinType joinType)
        {
            var conditionString = conditionList.Aggregate((a, b) => ToClauseConvention(a, b));
            return
                ToClauseConvention(
                    ToClauseConvention(
                        GetJoinClause(joinType),
                        ToTableNameConvention(entity)),
                    ToClauseConvention(
                        JoinConditionClause, 
                        conditionString)
                    );
        }

        public string Wrap(string entityName)
        {
            return ToEntityNameConvention(entityName);
        }

        public string GetJoinClause(JoinType joinType)
        {
            switch (joinType)
            {
                case JoinType.InnerJoin:
                    return "INNER JOIN";
                case JoinType.LeftJoin:
                    return "LEFT JOIN";
                default:
                    return string.Empty;
            }
        }

        public string ToSelectStar()
        {
            return ToClauseConvention(
                SelectClause, "*");
        }

        public string ToSelectClauseString(IEnumerable<Tuple<string, string>> clauseInfo)
        {
            return ToClauseConvention(
                SelectClause, 
                clauseInfo
                    .Select(t => ToQuantifiedNameConvention(t.Item1, t.Item2))
                    .Aggregate((a, b) => ToProjectionConvetion(a, b)));
        }

        public string ToWhereClauseString(IList<string> clauseList)
        {
            return ToClauseConvention(
                WhereClause,
                clauseList.Aggregate((a, b) => ToClauseConvention(a, b)));
        }

        //TODO:: update after implementing join
        public string ToFromClauseString(IList<Type> typeList)
        {
            throw new NotImplementedException();
        }

        public string ToFromClauseString(IEnumerable<string> tables)
        {
            return ToClauseConvention(FromClause, 
                tables.Aggregate((a,b) => ToProjectionConvetion(a,b)));
        }

        public string ToJoinConditionString(IList<string> clauseList, JoinType joinType)
        {
            return ToClauseConvention(JoinConditionClause,
                clauseList.Aggregate((a,b) => ToClauseConvention(a,b))
                );
        }
    }

    public  class MysqlConvention : BaseConvension
    {
        public MysqlConvention() : base()
        {
            EntityWrapper = "`";
        }

        public MysqlConvention(string entityWrapper)
        {
            EntityWrapper = entityWrapper;
        }
    }
}
