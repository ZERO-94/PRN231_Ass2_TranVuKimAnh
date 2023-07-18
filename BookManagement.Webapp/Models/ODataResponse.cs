using Newtonsoft.Json;

namespace BookManagement.Webapp.Models
{
    public class ODataResponse<T>
    {
        [JsonProperty("@odata.count")]
        public int? Count { get; set; }
        public T Value { get; set; }
    }
}
