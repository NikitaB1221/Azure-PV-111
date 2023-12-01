using Newtonsoft.Json;

namespace Azure_PV_111.Models.Home.Db
{
    public class Test
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey { get; set; }
        public string LastName { get; set; }

        public String Data { get; set; }
    }
}
