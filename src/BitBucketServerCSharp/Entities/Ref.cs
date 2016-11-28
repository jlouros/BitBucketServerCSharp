using Newtonsoft.Json;

namespace BitBucketServerCSharp.Entities
{
    public class Ref
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string DisplayId { get; set; }
        public string LatestChangeset { get; set; }
        [JsonProperty("repository")]
        public Repository Repository { get; set; }
    }
}
