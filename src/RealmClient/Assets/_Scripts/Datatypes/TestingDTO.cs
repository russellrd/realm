using Newtonsoft.Json;
using PocketBaseSdk;
public class TestingDTO : RecordModel
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("model")]
    public string model { get; set; }

    public static TestingDTO FromRecord(RecordModel record) => JsonConvert.DeserializeObject<TestingDTO>(record.ToString());
    public string ToRecord() => JsonConvert.SerializeObject(this);
}