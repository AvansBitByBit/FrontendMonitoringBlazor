using System.Text.Json.Serialization;

namespace FrontendMonitoring.Models
{
    public class LitterApiResponse
    {
        [JsonPropertyName("litter")]
        public List<AfvalModel>? Litter { get; set; }
        
        [JsonPropertyName("weather")]
        public string? Weather { get; set; }
    }
}
