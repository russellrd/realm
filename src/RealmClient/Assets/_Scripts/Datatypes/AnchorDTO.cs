using Newtonsoft.Json;
using PocketBaseSdk;
public class AnchorDTO : RecordModel
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("anchorId")]
    public string AnchorId { get; set; }

    [JsonProperty("userId")]
    public string UserId { get; set; }

    [JsonProperty("modelId")]
    public string ModelId { get; set; }

    [JsonProperty("scale")]
    public float Scale { get; set; }

    public static AnchorDTO FromRecord(RecordModel record) => JsonConvert.DeserializeObject<AnchorDTO>(record.ToString());
    public string ToRecord() => JsonConvert.SerializeObject(this);
}