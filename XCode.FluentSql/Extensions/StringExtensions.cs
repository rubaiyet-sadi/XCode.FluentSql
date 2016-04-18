using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace XCode.FluentSql.Extensions
{
    public static class StringExtensions
    {
        private static readonly PluralizationService PluralizationService;

        static StringExtensions()
        {
            PluralizationService = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));
        }

       

        public static string Pluralize(this string s)
        {
            if(s == null) throw new ArgumentNullException();
            return PluralizationService.Pluralize(s.ToLowerInvariant());
        }

        public static string Singularize(this string s)
        {
            if (s == null) throw new ArgumentNullException();
            return PluralizationService.Singularize(s.ToLowerInvariant());
        }

        public static bool IsNotNullOrEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
          
        }
    }

    
}
