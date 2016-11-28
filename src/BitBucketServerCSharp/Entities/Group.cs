using Newtonsoft.Json;

namespace BitBucketServerCSharp.Entities
{
    public class Group
    {
        [JsonProperty("name")]
        public string Name { get; set; }

    }
}
