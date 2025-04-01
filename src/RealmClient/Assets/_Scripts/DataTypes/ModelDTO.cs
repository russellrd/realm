using Newtonsoft.Json;
using PocketBaseSdk;
public class ModelDTO : RecordModel
{
    [JsonProperty("id")]
    public string ID { get; set; }

    [JsonProperty("creator")]
    public string Creator { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("model")]
    public string Model { get; set; }



    public static ModelDTO FromRecord(RecordModel record) => JsonConvert.DeserializeObject<ModelDTO>(record.ToString());
    public string ToRecord() => JsonConvert.SerializeObject(this);
}