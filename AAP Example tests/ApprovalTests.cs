using AAPExampleTests;
using ApprovalTests;
using ApprovalTests.Reporters;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace AAP_Example_tests
{   
    [UseReporter(typeof(DiffReporter))]
    public class ApprovalTests
    {
        [Fact]
        public void ApprovalTestswithVersionCompare()
        {
            var actual = Request.RestRequest("https://api.exchangeratesapi.io/", "2010-01-14", "GET");
            Approvals.Verify(ScrubDynamic(actual.Content));
        }

        [Fact]
        public void ApprovalTestswithVersionCompareDynamic()
        {
            var actual = Request.RestRequest("https://api.exchangeratesapi.io/", "2010-01-14", "GET");
            Approvals.Verify(ScrubDynamic(actual.Content));
        }


        private string ScrubDynamic(string resp)
        {
            string scrubbedResponse = Regex.Replace(resp, "\"date\":\"(.*?)\"", "###DATE");
            return scrubbedResponse;
        }
    }
}
