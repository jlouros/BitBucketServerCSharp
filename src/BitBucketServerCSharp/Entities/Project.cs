using Newtonsoft.Json;

namespace BitBucketServerCSharp.Entities
{
    //paged API https://developer.atlassian.com/static/rest/bitbucket-server/latest/bitbucket-rest.html#paging-params
    public class Project
    {
        public int Id { get; set; }
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        public bool Public { get; set; }
        public string Type { get; set; }
        public Link Link { get; set; }
        public Links Links { get; set; }
    }
}