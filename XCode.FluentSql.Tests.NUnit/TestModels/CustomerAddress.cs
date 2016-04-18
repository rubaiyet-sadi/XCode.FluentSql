using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCode.FluentSql.Tests.NUnit.TestModels
{
    public class CustomerAddress
    {
        public string AddressLineOne { get; set; }
        public string AddressLineTro { get; set; }
        public string AddressCity { get; set; }
        public string AddressState { get; set; }
        public string AddressZipCode { get; set; }
        public string AddressCounty { get; set; }
        public string AddressCountyCode { get; set; }
        public string AddressCountry { get; set; }
    }
}
