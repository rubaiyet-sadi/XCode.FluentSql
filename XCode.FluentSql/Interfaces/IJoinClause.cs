using System;
using System.Linq.Expressions;

namespace XCode.FluentSql.Interfaces
{
    public interface IJoinClause<T1, T2> : IClause<T1, T2>
    {
        IWhereClause<T1, T2> Where(Expression<Func<T1, T2, bool>> predicate);

    }

    public interface IJoinClause<T1, T2, T3> : IClause<T1, T2, T3>
    {

    }

    public interface IJoinClause<T1, T2, T3, T4> : IClause<T1,T2,T3,T4>
    {

    }
}
