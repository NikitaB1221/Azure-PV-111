using Newtonsoft.Json;

namespace Azure_PV_111.Models.Home.Db
{
    public class ProductDataModel
    {
        public const String DataType = "Product";

        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; } = null!;

        [JsonProperty(PropertyName = "year")]
        public String Year { get; set; } = null!;

        [JsonProperty(PropertyName = "type")]
        public String Type => DataType;
    }
}
