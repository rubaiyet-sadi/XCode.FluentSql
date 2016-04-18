namespace XCode.FluentSql.Interfaces
{
    public interface IWhereClause<T> : ISelectClause<T>
    {

    }

    public interface IWhereClause<T1, T2> : ISelectClause<T1, T2>, IJoinClause<T1,T2>
    {
        //ISelectClause<TLeft, TRight> Where(Expression<Func<TLeft, TRight, bool>> predicatExpression);
    }

    public interface IWhereClause<T1, T2, T3> : ISelectClause<T1, T2, T3>
    {

    }

    public interface IWhereClause<T1, T2, T3, T4> : ISelectClause<T1,T2,T3,T4>
    {

    }
}
