using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AAPExampleTests
{
    public static class CompareUtilities
    {

        /// <summary>
        /// Returns cached response from saved file. Expects file name and optional FilePath if saved in custom location
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <returns>string from json file</returns>
        public static string ReadCachedJson(string fileName, string filePath = "CachedResponses")
        {

            return File.ReadAllText($"{GetDirectory()}/{filePath}/{fileName}.json");
        }

        /// <summary>
        /// Saves to [BaseDirectory]/CachedResponse/[fileName provided] This is to be used with data compares later
        /// </summary>
        /// <param name="response"></param>
        /// <param name="fileName"></param>
        public static void SaveJsonResponse(IRestResponse response, string fileName, string filePath = "CachedResponses")
        {
            File.WriteAllText($"{GetDirectory()}/{filePath}/{fileName}.json", response.Content);
        }


        private static string GetDirectory()
        {
            var pathRegex = new Regex(@"\\bin(\\x86|\\x64)?\\(Debug|Release)?\\(netcoreapp3.1|netcoreapp3.0)$", RegexOptions.Compiled);
            return pathRegex.Replace(Directory.GetCurrentDirectory(), String.Empty);
        }

        /// <summary>
        /// Deep compare two NewtonSoft JObjects. If they don't match, returns text diffs
        /// </summary>
        /// <param name="source">The expected results</param>
        /// <param name="target">The actual results</param>
        /// <param name="ignoredFields">List of fields to ignore</param>
        /// <returns>Text string</returns>
        private static StringBuilder CompareObjects(JObject actual, JObject expected, List<string> ignoredFields = null)
        {
            StringBuilder returnString = new StringBuilder();
            foreach (KeyValuePair<string, JToken> sourcePair in actual)
            {
                if (ignoredFields != null && ignoredFields.Contains(sourcePair.Key))
                {
                    Console.WriteLine($"ignoring field {sourcePair.Key}");
                }
                else if (sourcePair.Value.Type == JTokenType.Object)
                {
                    if (sourcePair.Key == "NewField")
                    {
                        Console.WriteLine("here");
                    }
                    if (expected.GetValue(sourcePair.Key) == null)
                    {
                        returnString.Append("Key " + sourcePair.Key
                                            + " not found" + Environment.NewLine);
                    }
                    else if (expected.GetValue(sourcePair.Key).Type != JTokenType.Object)
                    {
                        returnString.Append("Key " + sourcePair.Key
                                            + " is not an object in expected" + Environment.NewLine);
                    }
                    else
                    {
                        returnString.Append(CompareObjects(sourcePair.Value.ToObject<JObject>(),
                            expected.GetValue(sourcePair.Key).ToObject<JObject>(), ignoredFields));
                    }
                }
                else if (sourcePair.Value.Type == JTokenType.Array)
                {
                    if (expected.GetValue(sourcePair.Key) == null)
                    {
                        returnString.Append("Key from actual " + sourcePair.Key
                                            + " not found in expected" + Environment.NewLine);
                    }
                    else
                    {
                        returnString.Append(CompareArrays(sourcePair.Value.ToObject<JArray>(),
                            expected.GetValue(sourcePair.Key).ToObject<JArray>(), sourcePair.Key, ignoredFields));
                    }
                }
                else
                {
                    JToken expectedVal = sourcePair.Value;
                    var actualVal = expected.SelectToken(sourcePair.Key);
                    if (actualVal == null)
                    {
                        returnString.Append("Key from actual: " + sourcePair.Key
                                            + " was removed from expected" + Environment.NewLine);
                    }
                    else
                    {
                        if (!JToken.DeepEquals(expectedVal, actualVal))
                        {
                            returnString.Append("Key " + sourcePair.Key + ": "
                                                + sourcePair.Value + " !=  "
                                                + expected.Property(sourcePair.Key).Value
                                                + Environment.NewLine);
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, JToken> expectedPair in expected)
            {
                if (ignoredFields != null && ignoredFields.Contains(expectedPair.Key))
                {
                    Console.WriteLine($"ignoring field {expectedPair.Key}");
                }
                else
                {
                    JToken expectedVal = expectedPair.Value;
                    var actualVal = actual.SelectToken(expectedPair.Key);
                    if (actualVal == null)
                    {
                        returnString.Append("Key " + expectedPair.Key
                                            + " was added to Actual" + Environment.NewLine);
                    }
                }
            }

            return returnString;
        }

        public static bool CompareObjects(IRestResponse actual, string expected, string testName, List<string> ignoredFields = null, string resultPath = "TestResults")
        {
            string results = CompareObjects(JObject.Parse(actual.Content), JObject.Parse(expected), ignoredFields).ToString();
            if (results == "")
            {
                return true;
            }
            else
            {
                File.WriteAllText($"{GetDirectory()}/{resultPath}/{testName}.txt", results);
                File.WriteAllText($"{GetDirectory()}/{resultPath}/failed-{testName}.json", actual.Content);
                return false;
            }

        }

        /// <summary>
        /// Deep compare two NewtonSoft JArrays. If they don't match, returns text diffs
        /// </summary>
        /// <param name="source">The expected results</param>
        /// <param name="target">The actual results</param>
        /// <param name="arrayName">The name of the array to use in the text diff</param>
        /// <returns>Text string</returns>

        public static StringBuilder CompareArrays(JArray source, JArray target, string arrayName = "", List<string> ignoredFields = null)
        {
            var returnString = new StringBuilder();
            for (var index = 0; index < source.Count; index++)
            {

                var expected = source[index];
                if (expected.Type == JTokenType.Object)
                {
                    var actual = (index >= target.Count) ? new JObject() : target[index];
                    returnString.Append(CompareObjects(expected.ToObject<JObject>(),
                        actual.ToObject<JObject>(), ignoredFields));
                }
                else
                {

                    var actual = (index >= target.Count) ? "" : target[index];
                    if (!JToken.DeepEquals(expected, actual))
                    {
                        if (String.IsNullOrEmpty(arrayName))
                        {
                            returnString.Append("Index " + index + ": " + expected
                                                + " != " + actual + Environment.NewLine);
                        }
                        else
                        {
                            returnString.Append("Key " + arrayName
                                                + "[" + index + "]: " + expected
                                                + " != " + actual + Environment.NewLine);
                        }
                    }
                }
            }
            return returnString;

        }
    }
}

