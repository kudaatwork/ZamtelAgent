using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YomoneyApp.Models
{
    public class RoutesInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("supplierId")]
        public string SupplierId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("serviceName")]
        public string ServiceName { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("customerMobileNumber")]
        public string CustomerMobileNumber { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("region_name")]
        public string Region { get; set; }

        [JsonProperty("country_name")]
        public string Country { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("time_zone")]
        public string TimeZone { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }
    }
}
