using System;

namespace XCode.FluentSql.Tests.NUnit.TestModels
{
    class CustomerOrders
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public Decimal UnitPrice { get; set; }
        public int UnitCount { get; set; }
    }
}
