using System;
using System.Linq.Expressions;

namespace XCode.FluentSql.Interfaces
{
    public interface ISelectClause<T> : IClause<T>
    {
        //ISelectClause<T> Select(Expression<Func<T, object>> expression);
        IWhereClause<T> Where(Expression<Func<T, bool>> predicatExpression);
        

    }

    public interface ISelectClause<T1, T2> : IClause<T1, T2>
    {
        IJoinClause<T1, T2> InnerJoin(Expression<Func<T1,T2,bool>> predicate);
        IWhereClause<T1, T2> Where(Expression<Func<T1, T2, bool>> predicate);
    }

    public interface ISelectClause<T1, T2, T3> : IClause<T1, T2, T3>
    {
        IJoinClause<T1, T2, T3> InnerJoin(
            Expression<Func<T1, T2, bool>> firstExpression,
            Expression<Func<T1, T3, bool>> sencondExpression = null);

        IJoinClause<T1, T2, T3> InnerJoin(
            Expression<Func<T1, T3, bool>> firstExpression,
            Expression<Func<T1, T2, bool>> sencondExpression = null);


        IWhereClause<T1, T2, T3> Where(Expression<Func<T1, T2, T3, bool>> predicate);
    }

    public interface ISelectClause<T1, T2, T3, T4> : IClause<T1, T2, T3, T4>
    {
        IJoinClause<T1, T2, T3, T4> InnerJoin(Expression<Func<T1, T2, T3, T4, bool>> predicate);
        IWhereClause<T1, T2, T3, T4> Where(Expression<Func<T1, T2, T3, T4, bool>> predicate);
    }
}
