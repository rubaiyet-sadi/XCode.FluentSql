using System;
using System.Linq.Expressions;

namespace XCode.FluentSql.Interfaces
{
    public interface IClause<T>
    {
        ISelectClause<T> Select();
        ISelectClause<T> Select(Expression<Func<T, object>> expression);
    }

    public interface IClause<T1,T2>
    {
        ISelectClause<T1, T2> Select();
        ISelectClause<T1, T2> Select(Expression<Func<T1, T2, object>> expression);
    }

    public interface IClause<T1, T2,T3>
    {
        ISelectClause<T1, T2, T3> Select(Expression<Func<T1, T2, T3, object>> expression);
    }

    public interface IClause<T1, T2, T3, T4>
    {
        ISelectClause<T1, T2, T3, T4> Select(Expression<Func<T1, T2, T3, T4, object>> expression);
    }
}
