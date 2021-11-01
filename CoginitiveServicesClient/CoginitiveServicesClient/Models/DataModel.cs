using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CoginitiveServicesClient.Models
{
    public class DataModel
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("sendToEmail")]
        public string SendToEmail { get; set; }

        [JsonPropertyName("image1")]
        public string Image1 { get; set; }

        [JsonPropertyName("image2")]
        public string Image2 { get; set; }

    }
}
