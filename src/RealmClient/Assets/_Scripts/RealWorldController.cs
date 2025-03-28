using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RealWorldController : MonoBehaviour
{
    private const int MAX_WAIT_TIME = 15;
    private const int WEATHER_UPDATE_PERIOD_MINUTES = 5;
    private readonly Dictionary<int, string> HAZARD_WEATHER_CODES = new()
    {
        {0, "TEST HAZARD0"},
        {1, "TEST HAZARD1"},
        {2, "TEST HAZARD2"},
        {3, "TEST HAZARD3"},
        {57, "Freezing Drizzle"},
        {67, "Freezing Rain"},
        {74, "Heavy Snow"},
        {75, "Heavy Snow"},
        {79, "Ice Pellets"},
        {82, "Heavy Showers"},
        {97, "Thunderstorm Without Hail"},
        {99, "Thunderstorm With Hail"}
    };

    [SerializeField]
    private PopupManager popupManager;

    [HideInInspector]
    public WeatherDTO weather;

    [HideInInspector]
    public GPSCoordinate gps;

    private bool gpsOn = false;

    private float timer = 0;

    IEnumerator Start()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("Location not enabled");
            popupManager.ShowErrorPopup("Location not enabled");
        }

        Input.location.Start();

        int waitTime = 0;
        while (Input.location.status == LocationServiceStatus.Initializing & waitTime <= MAX_WAIT_TIME)
        {
            yield return new WaitForSeconds(1);
            waitTime++;
        }

        if (waitTime > MAX_WAIT_TIME)
        {
            Debug.LogError("Could not connect to GPS");
            popupManager.ShowErrorPopup("Could not connect to GPS");
            yield break;
        }

        gpsOn = true;

        gps = new GPSCoordinate(Input.location.lastData.latitude, Input.location.lastData.longitude);

        Debug.Log("INITIAL GPS: " + Input.location.lastData.longitude + " " + Input.location.lastData.latitude);

        StartCoroutine(GetWeather());
    }

    void Update()
    {
        if (gpsOn)
        {
            if (timer > WEATHER_UPDATE_PERIOD_MINUTES * 60)
            {
                StartCoroutine(GetWeather());
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }

            gps.Latitude = Input.location.lastData.latitude;
            gps.Longitude = Input.location.lastData.longitude;
        }
    }

    public IEnumerator GetWeather()
    {
        if (!gpsOn)
        {
            Debug.LogError("Could not get weather because GPS is not on");
            popupManager.ShowErrorPopup("GPS not on");
            yield break;
        }

        using UnityWebRequest www = UnityWebRequest.Get($"https://api.open-meteo.com/v1/forecast?latitude={Input.location.lastData.latitude}&longitude={Input.location.lastData.longitude}&hourly=weather_code,temperature_2m,precipitation,precipitation_probability&current=temperature_2m,weather_code,cloud_cover,precipitation&timezone=auto");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("SUCCESS: " + www.downloadHandler.text);
            weather = WeatherDTO.FromJSON(www.downloadHandler.text);
            Debug.Log($"Current Temperature (C): {weather.Current.Temperature}");
            Debug.Log($"Current Weather Code: {weather.Current.WeatherCode}");
            CheckForWeatherHazards();
        }
        else
        {
            Debug.LogError("Could not get weather: " + www.error);
            popupManager.ShowErrorPopup("Could not get weather");
        }
    }

    public GPSCoordinate GetCurrentGPSCoordinates()
    {
        return gps.Clone();
    }

    private void StopGPS()
    {
        Input.location.Stop();
    }

    private void CheckForWeatherHazards()
    {
        if (HAZARD_WEATHER_CODES.TryGetValue(weather.Current.WeatherCode, out string codeMsg))
        {
            Debug.Log("WEATHER HAZARD: " + codeMsg);
            popupManager.ShowWeatherHazardPopup(codeMsg);
        }
    }
}

public class GPSCoordinate
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public GPSCoordinate()
    {
        Latitude = 0;
        Longitude = 0;
    }

    public GPSCoordinate(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public GPSCoordinate Clone()
    {
        return new GPSCoordinate(Latitude, Longitude);
    }

    public override string ToString()
    {
        return $"({Latitude}, {Longitude})";
    }
}