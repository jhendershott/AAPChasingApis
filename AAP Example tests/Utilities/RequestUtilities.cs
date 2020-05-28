using System;
using System.Collections.Generic;
using RestSharp;
using System.Net;

namespace AAPExampleTests
{
    public class Request
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="endpoint"></param>
        /// <param name="verb"></param>
        /// <param name="client"></param>
        /// <param name="headers"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IRestResponse RestRequest(string url, string endpoint, string verb, RestClient client = null, Dictionary<string, string> headers = null, Dictionary<string, string> parameters = null, string body = null, string contentType = "json")
        {
            if (client == null)
            {
                client = new RestClient(url);
            }
            IRestResponse response = null;
            RestRequest request = new RestRequest(endpoint);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    request.AddParameter(param.Key, param.Value);
                }
            }

            if (body != null)
            {
                request.AddParameter($"application/{contentType}", body, ParameterType.RequestBody);
            }

            try
            {
                if (verb.ToLower().Equals("get"))
                {
                    response = client.Get(request);
                }
                else if (verb.ToLower().Equals("post"))
                {
                    response = client.Post(request);
                }
                else if (verb.ToLower().Equals("patch"))
                {
                    response = client.Patch(request);
                }
                else if (verb.ToLower().Equals("put"))
                {
                    response = client.Put(request);
                }
                else if (verb.ToLower().Equals("delete"))
                {
                    response = client.Delete(request);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong with your request: " + e.InnerException);
            }

            return response;
        }

        public static bool StatusOkay(IRestResponse response)
        {
            bool okay = true;
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine("Request was not completed Status: " + response.StatusCode);
                Console.WriteLine("Issue: " + response.ErrorMessage);
                okay = false;
            }

            return okay;
        }
    }
}
