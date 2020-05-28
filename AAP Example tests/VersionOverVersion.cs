using AAPExampleTests;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AAP_Example_tests
{
    public class VersionOverVersion
    {
        [Fact]
        public void TestwithJsonCompareShouldPass()
        {
            var response = Request.RestRequest("https://api.exchangeratesapi.io/", "2010-01-14", "GET");
            Assert.True(CompareUtilities.CompareObjects(response, CompareUtilities.ReadCachedJson("CurrencyByDate2010-01-14"), "Date2010-01-14"));
        }

        [Fact]
        public void TestswithJsonCompareFail()
        {
            var response = Request.RestRequest("https://api.exchangeratesapi.io/", "2010-01-14", "GET");

            List<string> ignoredFields = new List<string> { "date" };
            

            Assert.True(CompareUtilities.CompareObjects(response, CompareUtilities.ReadCachedJson("CurrencyByDate2010-01-15"), "Date2010-01-15"));
        }

        [Fact]
        public void TestwithLotsofFailures()
        {
            var response = Request.RestRequest("https://api.exchangeratesapi.io/", "2010-01-14", "GET");
            List<string> ignoredFields = new List<string> { "date" };
            Assert.True(CompareUtilities.CompareObjects(response, CompareUtilities.ReadCachedJson("NewObjCurrencyByDate2010-01-15"), "Date2010-01-15-failures", ignoredFields));
        }

        [Fact]
        public void TestswithJsonCompareVersions()
        {
            var actual = Request.RestRequest("https://api.exchangeratesapi.io/", "2010-01-14", "GET");
            var expected = Request.RestRequest("https://api.exchangeratesapi.io/", "2010-01-15", "GET");
            Assert.True(CompareUtilities.CompareObjects(actual, expected.Content, "CompareVersions"));
        }


    }
}
