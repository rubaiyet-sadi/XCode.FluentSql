using XCode.FluentSql.Interfaces;

namespace XCode.FluentSql
{
    public interface IQueryBuilder<T> : IWhereClause<T>
    {
    }

    public interface IQueryBuilder<T1, T2> : IWhereClause<T1,T2>
    {
    }

    public interface IQueryBuilder<T1, T2, T3> : IWhereClause<T1, T2, T3>
    {
    }
    public interface IQueryBuilder<T1, T2, T3, T4> : IWhereClause<T1, T2, T3, T4>
    {
    }
}
