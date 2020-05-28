
using Newtonsoft.Json.Linq;
using System;
using Xunit;

namespace AAPExampleTests
{
    public class TestingCodesAndFields
    {
        [Fact]
        public void TestStatusCode200()
        {
            var response = Request.RestRequest("https://api.exchangeratesapi.io/", "2010-01-14", "GET");
            Assert.True(Request.StatusOkay(response));
        }

        [Fact]
        public void TestStatusCode400()
        {
            var response = Request.RestRequest("https://api.exchangeratesapi.io/", "abc2010-01-14", "GET");
            Assert.False(Request.StatusOkay(response));
            Assert.Equal("Bad Request", response.StatusDescription);
        }

        [Fact]
        public void TestsWithDynamicObject()
        {
            var response = Request.RestRequest("https://api.exchangeratesapi.io/", "2010-01-14", "GET");
            dynamic ratesObject = JObject.Parse(response.Content);
            Assert.Equal("2010-01-14", ratesObject.date.ToString());
            Assert.Equal(1.4942, (double)ratesObject.rates.CAD);
        }

        [Fact]
        public void TestsWithDeserializedObject()
        {
            var response = Request.RestRequest("https://api.exchangeratesapi.io/", "2010-01-14", "GET");
            CurrencyRateByDate ratesObject = CurrencyRateByDate.FromJson(response.Content);
            Assert.Equal("2010-01-14", ratesObject.Date);
            Assert.Equal(1.4942, ratesObject.Rates["CAD"]);
        }


       
    }
}
