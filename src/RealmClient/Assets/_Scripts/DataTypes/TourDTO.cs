using Newtonsoft.Json;
using PocketBaseSdk;

public class TourDTO : RecordModel
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("organizationId")]
    public string OrganizationId { get; set; }

    [JsonProperty("startLatitude")]
    public double StartLatitude { get; set; }

    [JsonProperty("startLongitude")]
    public double StartLongitude { get; set; }

    public static TourDTO FromRecord(RecordModel record) => JsonConvert.DeserializeObject<TourDTO>(record.ToString());
    public string ToRecord() => JsonConvert.SerializeObject(this);
}