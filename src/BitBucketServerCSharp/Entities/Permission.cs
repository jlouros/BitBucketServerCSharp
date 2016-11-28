using Newtonsoft.Json;

namespace BitBucketServerCSharp.Entities
{
    public class Permission
    {
        [JsonProperty("group")]
        public Group Group { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("permission")]
        public string permission { get; set; }
    }
}
