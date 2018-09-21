using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OCRAPITest
{
    public partial class ReceiptData
    {
        [JsonProperty("receiptCategory")]
        public string ReceiptCategory { get; set; }

        [JsonProperty("merchant")]
        public string VendorName { get; set; }

        [JsonProperty("maxTotalAmount1")]
        public string TotalAmount1 { get; set; }

        [JsonProperty("totalAmount2")]
        public string TotalAmount2 { get; set; }

        [JsonProperty("receiptDate")]
        public string ReceiptDate { get; set; }
    }

    public partial class ReceiptData
    {
        public static ReceiptData FromJson(string json) => JsonConvert.DeserializeObject<ReceiptData>(json, OCRAPITest.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ReceiptData self) => JsonConvert.SerializeObject(self, OCRAPITest.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
