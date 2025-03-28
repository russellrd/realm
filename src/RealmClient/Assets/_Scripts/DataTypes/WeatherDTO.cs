using System;
using Newtonsoft.Json;

public class WeatherDTO
{
    [JsonProperty("latitude")]
    public double Latitude { get; set; }

    [JsonProperty("longitude")]
    public double Longitude { get; set; }

    [JsonProperty("timezone")]
    public string Timezone { get; set; }

    [JsonProperty("timezone_abbreviation")]
    public string TimezoneAbbreviation { get; set; }

    [JsonProperty("elevation")]
    public string Elevation { get; set; }

    [JsonProperty("current")]
    public Current Current { get; set; }

    public static WeatherDTO FromJSON(string json) => JsonConvert.DeserializeObject<WeatherDTO>(json.ToString());
}

public class Current
{
    [JsonProperty("time")]
    public DateTime Time { get; set; }

    [JsonProperty("interval")]
    public int Interval { get; set; }

    [JsonProperty("temperature_2m")]
    public double Temperature { get; set; }

    [JsonProperty("weather_code")]
    public int WeatherCode { get; set; }

    [JsonProperty("cloud_cover")]
    public double CloudCover { get; set; }

    [JsonProperty("precipitation")]
    public double Precipitation { get; set; }
}