// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Microsoft.Bot.Connector
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    public partial class APIResponse
    {
        /// <summary>
        /// Initializes a new instance of the APIResponse class.
        /// </summary>
        public APIResponse() { }

        /// <summary>
        /// Initializes a new instance of the APIResponse class.
        /// </summary>
        public APIResponse(string message = default(string))
        {
            Message = message;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

    }
}
