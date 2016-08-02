using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Shouldly;
using XCode.FluentSql.Extensions;
using XCode.FluentSql.Tests.NUnit.TestModels;

namespace XCode.FluentSql.Tests.NUnit
{
    [TestFixture]
    public class SqlTests
    {
        [Test]
        public void ToStringWithoutSelectException()
        {
            var expected = QueryFactory.AsMySqlQuery<Customer>();

            Should.Throw<InvalidOperationException>(() => expected.ToString());
        }

        [Test]
        public void OneTypeWithoutWhere()
        {
            var generatedSql = QueryFactory.AsMySqlQuery<Customer>().Select(c => new
            {
                c.Id,
                c.FirstName,
                c.MiddleInitial,
                c.LastName,
                c.DateofBirth,
                c.SSN
            })
                .ToString();
            var expectedSql =AsSqlString(
                "SELECT `customers`.`Id`, `customers`.`FirstName`, `customers`.`MiddleInitial`, `customers`.`LastName`, `customers`.`DateofBirth`, `customers`.`SSN`", 
                "FROM `customers`",
                "");
            generatedSql.ShouldBe(expectedSql);

        }
        [Test]
        public void OneTypeWithWhere()
        {
            var generatedSql = QueryFactory.AsMySqlQuery<Customer>().Select(c => new
            {
                c.Id,
                c.FirstName,
                c.MiddleInitial,
                c.LastName,
                c.DateofBirth,
                c.SSN
            })
                .Where(c => c.FirstName == "AA" && c.LastName == "BB")
                .ToString();

            var expectedSql = AsSqlString(
                "SELECT `customers`.`Id`, `customers`.`FirstName`, `customers`.`MiddleInitial`, `customers`.`LastName`, `customers`.`DateofBirth`, `customers`.`SSN`",
                "FROM `customers`",
                "WHERE `customers`.`FirstName`=`AA` AND `customers`.`LastName`=`BB`");

            generatedSql.ShouldBe(expectedSql);

        }

        [Test]
        public void OneTypeWithWhereWithVariable()
        {
            var testVar = "AA";
            var generatedSql = QueryFactory.AsMySqlQuery<Customer>().Select(c => new
            {
                c.Id,
                c.FirstName,
                c.MiddleInitial,
                c.LastName,
                c.DateofBirth,
                c.SSN
            })
                .Where(c => c.FirstName == testVar && c.LastName == "BB")
                .ToString();

            var expectedSql = AsSqlString(
                "SELECT `customers`.`Id`, `customers`.`FirstName`, `customers`.`MiddleInitial`, `customers`.`LastName`, `customers`.`DateofBirth`, `customers`.`SSN`",
                "FROM `customers`",
                "WHERE `customers`.`FirstName`=`AA` AND `customers`.`LastName`=`BB`");

            generatedSql.ShouldBe(expectedSql);

        }

        [Test]
        public void SelectStarWithoutWhere()
        {
            var generatedSql = QueryFactory.AsMySqlQuery<Customer>().Select().ToString();
            var expectedSql = AsSqlString(
               "SELECT *",
               "FROM `customers`",
               "");

            generatedSql.ShouldBe(expectedSql);

        }

        [Test]
        public void JoinClauseWithoutWhere()
        {
            var generatedSql = QueryFactory
                .AsMySqlQuery<Customer, CustomerOrders>()
                .Select()
                .InnerJoin((c, o) => c.Id == o.CustomerId)
                .ToString();
            var expectedSql = AsSqlString(
                "SELECT *",
                "FROM `customers`",
                new List<string>() {"INNER JOIN `customerorders` ON `customers`.`Id`=`customerorders`.`CustomerId`"},
                "");
            generatedSql.ShouldBe(expectedSql);

        }

        [Test]
        public void JoinClauseWithWhere()
        {
            var generatedSql = QueryFactory
                .AsMySqlQuery<Customer, CustomerOrders>()
                .Select()
                .InnerJoin((c, o) => c.Id == o.CustomerId)
                .Where((c,o) => c.MiddleInitial == "A" )
                .ToString();
            var expectedSql = AsSqlString(
                "SELECT *",
                "FROM `customers`",
                new List<string>() { "INNER JOIN `customerorders` ON `customers`.`Id`=`customerorders`.`CustomerId`" },
               "WHERE `customers`.`MiddleInitial`=`A`");
            //Assert.AreEqual(generatedSql, expectedSql);
            generatedSql.ShouldBe(expectedSql);
        }

        [Test]
        public void SelectClauseWithThreeEntity()
        {
            var generatedSql = QueryFactory
                .AsMySqlQuery<Customer, CustomerOrders, CustomerAddress>()
                .Select((customer, orders, address) => new
                {
                    customer.Id,
                    customer.FirstName,
                    orders.OrderDate,
                    address.AddressState
                })
                .ToString();
            var expectedSql = AsSqlString(
                "SELECT `customers`.`Id`, `customers`.`FirstName`, `customerorders`.`OrderDate`, `customeraddresses`.`AddressState`",
                "FROM `customers`, `customerorders`, `customeraddresses`",
                "");
            generatedSql.ShouldBe(expectedSql);
        }

        private string AsSqlString(string selectCluaseString, string fromClauseString, List<string> joinClauseList, string whereClauseString)
        {
            var sqlBuilderString = new StringBuilder();
            sqlBuilderString
                .AppendLine(selectCluaseString)
                .AppendLine(fromClauseString);
            foreach (var joinClause in joinClauseList)
            {
                sqlBuilderString.AppendLine(joinClause);
            }
            sqlBuilderString
                .AppendFormat(whereClauseString.IsNotNullOrEmpty() ? whereClauseString : string.Empty);
            return sqlBuilderString.ToString();
        }

        private string AsSqlString(string selectCluaseString, string fromClauseString, string whereClauseString)
        {
            var sqlBuilderString = new StringBuilder();
            sqlBuilderString
                .AppendLine(selectCluaseString)
                .AppendLine(fromClauseString)
                .AppendFormat(whereClauseString.IsNotNullOrEmpty() ? whereClauseString : string.Empty);
            return sqlBuilderString.ToString();
        }
    }
}
