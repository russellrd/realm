using Newtonsoft.Json;
using PocketBaseSdk;

public class OrganizationDTO : RecordModel
{
    [JsonProperty("name")]
    public string Name { get; set; }

    public static OrganizationDTO FromRecord(RecordModel record) => JsonConvert.DeserializeObject<OrganizationDTO>(record.ToString());
    public string ToRecord() => JsonConvert.SerializeObject(this);
}