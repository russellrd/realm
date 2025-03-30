using Newtonsoft.Json;

public class UserCreateDTO
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }

    [JsonProperty("passwordConfirm")]
    public string PasswordConfirm { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("organizationId")]
    public string OrganizationId { get; set; }

    public string ToRecord() => JsonConvert.SerializeObject(this);
}