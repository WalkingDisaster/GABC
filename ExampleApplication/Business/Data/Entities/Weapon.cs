using Newtonsoft.Json;

namespace ExampleApplication.Business.Data.Entities
{
    public class Weapon
    {
        [JsonProperty("name")]
        public string Name { get; set; } 

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}