using Newtonsoft.Json;

namespace Azure_PV_111.Models.Home.Db
{
    public class ProducerDataModel
    {
        [JsonProperty(PropertyName ="id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; } = null!;
        
        [JsonProperty(PropertyName = "type")]
        public String Type => "Producer"; 

        [JsonProperty(PropertyName = "partitionKey")]
        public String PartitionKey => "Producer";

        
    }
}
