using XCode.FluentSql.DbConvension;
using XCode.FluentSql.Interfaces;

namespace XCode.FluentSql
{
    //TODO:: use DI to inject dbconvention
    public static class QueryFactory
    {
        public static IClause<T> AsMySqlQuery<T>()
        {
            BaseConvension convension = new MysqlConvention();
            return new QueryBuilder<T>(convension);
        }

        public static IClause<T1, T2> AsMySqlQuery<T1, T2>()
        {
            BaseConvension convension = new MysqlConvention();
            return new QueryBuilder<T1, T2>(convension);
        }
        public static IClause<T1, T2, T3> AsMySqlQuery<T1, T2, T3>()
        {
            BaseConvension convension = new MysqlConvention();
            return new QueryBuilder<T1, T2, T3>(convension);
        }
    }
}
