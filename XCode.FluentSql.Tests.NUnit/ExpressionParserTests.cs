using System;
using System.Linq.Expressions;
using NUnit.Framework;
using XCode.FluentSql.DbConvension;
using XCode.FluentSql.Extensions;
using XCode.FluentSql.Tests.NUnit.TestModels;

namespace XCode.FluentSql.Tests.NUnit
{
    [TestFixture]
    public class ExpressionParserTests
    {
        [Test]
        public void AnonymousObjectTest()
        {
            var dbConvension = new MysqlConvention();
            Expression<Func<Customer, object>> selectClause = customer =>
            new {
                customer.Id,
                customer.LastName
            };
            var generated = selectClause.ToSelectClause(dbConvension);

            var expected = "SELECT `customers`.`Id`, `customers`.`LastName`";
            Assert.AreEqual(generated, expected);

        }
        [Test]
        public void AnonymousObjectNamedPropertyTest()
        {
            var dbConvension = new MysqlConvention();
            Expression<Func<Customer, object>> selectClause = customer =>
            new
            {
                customerId = customer.Id,
                customerLastName = customer.LastName
            };
            var generated = selectClause.ToSelectClause(dbConvension);

            var expected = "SELECT `customers`.`Id`, `customers`.`LastName`";
            Assert.AreEqual(generated, expected);

        }

        [Test]
        public void SinglePredicateEqualsValueTest()
        {
            var randomGuid = Guid.NewGuid();
            Expression<Func<Customer, bool>> findCustomer = customer => customer.LastName == "AA";
            var generated = findCustomer.BuildWhereClauseString(new MysqlConvention());

            var expected =
                "WHERE `customers`.`LastName`=`AA`";


            Assert.AreEqual(generated, expected);
        }

        [Test]
        public void SinglePredicateEqualsObjectValueTest()
        {
            var randomGuid = Guid.Empty;
            Expression<Func<Customer, bool>> findCustomer = customer => customer.Id == randomGuid;
            var generated = findCustomer.BuildWhereClauseString(new MysqlConvention());

            var expected =
                string.Format("WHERE `customers.Id=`{0}`", randomGuid.ToString());
            

            //Assert.AreEqual(generated, expected);
        }
    }
}
