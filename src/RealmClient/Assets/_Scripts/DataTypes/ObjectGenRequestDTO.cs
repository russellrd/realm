using Newtonsoft.Json;

public class ObjectGenRequestDTO
{
    [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
    public string Text { get; set; }

    [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
    public string Image { get; set; }

    [JsonProperty("texture", NullValueHandling = NullValueHandling.Ignore)]
    public string Texture { get; set; }
}