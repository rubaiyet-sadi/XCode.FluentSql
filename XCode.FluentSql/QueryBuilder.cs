using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using XCode.FluentSql.DbConvension;
using XCode.FluentSql.Enums;
using XCode.FluentSql.Extensions;
using XCode.FluentSql.Interfaces;

namespace XCode.FluentSql
{
    public class QueryBuilder
    {
        protected readonly BaseConvension DbConvension;
        protected string SelectCluaseString;
        protected string FromClauseString;
        protected string WhereClauseString;
        protected IList<string> JoinClauseList;
        protected string GroupByClauseString;
        protected string HavingClauseString;
        protected string OrderByClauseString;

       

        public QueryBuilder()
        {
            SelectCluaseString = string.Empty;
            FromClauseString = string.Empty;
            WhereClauseString = string.Empty;
            JoinClauseList = new List<string>();
        }
        public QueryBuilder(BaseConvension dbConvension) : this()
        {
            DbConvension = dbConvension;
        }

        public void SetSelect()
        {
            SelectCluaseString = DbConvension.ToSelectStar();
        }

        public void SetSelect(Expression expression)
        {
            var lambda = expression as LambdaExpression;
            if (lambda == null) throw new ArgumentNullException();

            SelectCluaseString = lambda.ToSelectClause(DbConvension);
        }

        public void SetFrom(string[] tables)
        {
            FromClauseString = DbConvension.ToFromClauseString(tables.Select(t => DbConvension.ToTableNameConvention(t)));
        }

        public void SetWhere(Expression expression)
        {
            if (expression == null) throw new ArgumentNullException();
            WhereClauseString = expression.BuildWhereClauseString(DbConvension);
        }

        public void SetJoin(Expression expression, string entity, JoinType joinType)
        {
            if(expression == null) throw new ArgumentNullException();

            var joinCondition = expression.PredicateParser(DbConvension, false);

            JoinClauseList.Add(
                DbConvension.ToJoinClauseConvention(entity, joinCondition, joinType));
        }
        public override string ToString()
        {
            if(string.IsNullOrEmpty(SelectCluaseString))
                throw new InvalidOperationException("To string cannot be called before setting select caluse");

            //TODO:: move to Template
            var sqlBuilderString = new StringBuilder();
            sqlBuilderString
                .AppendLine(SelectCluaseString)
                .AppendLine(FromClauseString);

            foreach (var joinClause in JoinClauseList)
            {
                sqlBuilderString.AppendLine(joinClause);
            }
            
            sqlBuilderString    
                .AppendFormat(WhereClauseString.IsNotNullOrEmpty() ? WhereClauseString : string.Empty);
            
            return sqlBuilderString.ToString();
        }
    }
    public class QueryBuilder<T> : QueryBuilder, IQueryBuilder<T>
    {
        public QueryBuilder(BaseConvension convension) : base(convension)
        {
            SetFrom(
                new string[]
                {
                    typeof (T).Name
                });
        }

        public ISelectClause<T> Select()
        {
            SetSelect();
            return this;
        }
        public ISelectClause<T> Select(Expression<Func<T, object>> expression)
        {
            SetSelect(expression);
            return this;
        }

        public IWhereClause<T> Where(Expression<Func<T, bool>> predicatExpressions)
        {
            SetWhere(predicatExpressions);
            return this;
        }
    }

    public class QueryBuilder<T1, T2> : QueryBuilder, IQueryBuilder<T1, T2>
    {

        public List<string> SelectClauseList { get; set; }
        public QueryBuilder(BaseConvension convension) : base(convension)
        {
            // TODO: Complete member initialization
            SetFrom(
                new string[]
                {
                    typeof (T1).Name,
                    typeof (T2).Name

                });
        }

        public ISelectClause<T1, T2> Select()
        {
            SetSelect();
            return this;
        }
        public ISelectClause<T1, T2> Select(Expression<Func<T1, T2, object>> expression)
        {
            SetSelect(expression);
            return this;
        }

        public IJoinClause<T1, T2> InnerJoin(Expression<Func<T1, T2, bool>> expression)
        {
            SetFrom(new string[]
            {
                typeof(T1).Name
            });
            SetJoin(expression, typeof(T2).Name, JoinType.InnerJoin);
            return this;
        }

        public IWhereClause<T1, T2> Where(Expression<Func<T1, T2, bool>> expression)
        {
            SetWhere(expression);
            return this;
        }
    }

    public class QueryBuilder<T1, T2, T3> : QueryBuilder, IQueryBuilder<T1, T2, T3>
    {
        public QueryBuilder(BaseConvension convension)
            : base(convension)
        {
            SetFrom(
                new string[]
                {
                    typeof (T1).Name,
                    typeof (T2).Name,
                    typeof (T3).Name
                });
        }

        public ISelectClause<T1, T2, T3> Select()
        {
            SetSelect();
            return this;
        }
        public ISelectClause<T1, T2, T3> Select(Expression<Func<T1, T2, T3, object>> expression)
        {
            SetSelect(expression);
            return this;
        }

        public IJoinClause<T1, T2, T3> InnerJoin(
            Expression<Func<T1, T2, bool>> firstExpression, 
            //Expression<Func<T2, T3, bool>> secondExpression = null,
            Expression<Func<T1, T3, bool>> secondExpression = null)
        {
            SetFrom(new string[]
            {
                typeof(T1).Name
            });
            SetJoin(firstExpression, typeof(T2).Name, JoinType.InnerJoin);
            SetJoin(secondExpression, typeof(T3).Name, JoinType.InnerJoin);
            return this as IJoinClause<T1, T2, T3>;
        }

        public IJoinClause<T1, T2, T3> InnerJoin(
            Expression<Func<T1, T3, bool>> firstExpression,
            Expression<Func<T1, T2, bool>> sencondExpression = null)
        {
            throw new NotImplementedException();
        }

        public IWhereClause<T1, T2, T3> Where(Expression<Func<T1, T2, T3, bool>> expression)
        {
            SetWhere(expression);
            return this;
        }
    }

}
