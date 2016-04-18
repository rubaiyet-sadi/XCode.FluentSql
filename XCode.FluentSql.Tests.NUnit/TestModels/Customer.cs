using System;

namespace XCode.FluentSql.Tests.NUnit.TestModels
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public DateTime DateofBirth { get; set; }
        public string SSN { get; set; }


        public static Customer GetRandomCustomer()
        {
            return new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = "",
                LastName =  "",
                DateofBirth = DateTime.Now
            };
        }
    
    }
}
