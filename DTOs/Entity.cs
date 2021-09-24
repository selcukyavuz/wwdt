using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Company.Function
{
    public class Entity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "dataType")]
        public DataType DataType { get; set; }

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }
    }
}