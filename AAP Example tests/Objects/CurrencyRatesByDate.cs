
using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace AAPExampleTests 
{

    public partial class CurrencyRateByDate
    {
        [JsonProperty("rates")]
        public Dictionary<string, double> Rates { get; set; }

        [JsonProperty("base")]
        public string Base { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }
    }

    public partial class CurrencyRateByDate
    {
        public static CurrencyRateByDate FromJson(string json) => JsonConvert.DeserializeObject<CurrencyRateByDate>(json, AAPExampleTests.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this CurrencyRateByDate self) => JsonConvert.SerializeObject(self, AAPExampleTests.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
