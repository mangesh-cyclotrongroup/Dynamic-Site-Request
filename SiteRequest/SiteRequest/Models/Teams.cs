using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteRequest.Dialogs
{
    public class Teams
    {
        [JsonIgnore]
        public const string TYPE = "Teams";

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = TYPE;

        [JsonProperty(PropertyName = "TeamName")]
        public string TeamName { get; set; }

        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "TeamMailNickname")]
        public string TeamMailNickname { get; set; }

        [JsonProperty(PropertyName = "TeamOwners")]
        public string TeamOwners { get; set; }

        [JsonProperty(PropertyName = "TeamType")]
        public string TeamType { get; set; }

        [JsonProperty(PropertyName = "Classification")]
        public string Classification { get; set; }
    }
}