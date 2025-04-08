using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Realm
{
    public class RealWorldController : MonoBehaviour
    {
        private const int MAX_WAIT_TIME = 15;
        private const int WEATHER_UPDATE_PERIOD_MINUTES = 5;
        private const int TOUR_PROXIMITY_UPDATE_PERIOD_SECONDS = 10;
        private const int TOUR_PROXIMITY_DISTANCE_METERS = 50;

        private readonly Dictionary<int, string> HAZARD_WEATHER_CODES = new()
        {
            // {0, "TEST HAZARD0"},
            // {1, "TEST HAZARD1"},
            // {2, "TEST HAZARD2"},
            // {3, "TEST HAZARD3"},
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
        public static GPSCoordinate gps;

        private bool gpsOn = false;

        private float weatherCheckTimer = 0;
        private float tourProximityCheckTimer = 0;

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
                UpdateCheckWeather();
                UpdateCheckTourProximity();

                if (Input.location.status == LocationServiceStatus.Running)
                {
                    gps.Latitude = Input.location.lastData.latitude;
                    gps.Longitude = Input.location.lastData.longitude;
                }
            }
        }

        private void UpdateCheckWeather()
        {
            if (weatherCheckTimer % (WEATHER_UPDATE_PERIOD_MINUTES * 60) == 0)
                StartCoroutine(GetWeather());
            weatherCheckTimer += Time.deltaTime;
        }

        private void UpdateCheckTourProximity()
        {
            if (tourProximityCheckTimer % TOUR_PROXIMITY_UPDATE_PERIOD_SECONDS == 0)
                StartCoroutine(GetTourProximity());
            tourProximityCheckTimer += Time.deltaTime;
        }

        public IEnumerator GetWeather()
        {
            if (!gpsOn)
            {
                Debug.LogError("Could not get weather because GPS is not on");
                popupManager.ShowErrorPopup("GPS not on");
                yield break;
            }

            GPSCoordinate coordinate = new();
            if (Input.location.lastData.latitude == 0 || Input.location.lastData.longitude == 0)
            {
                coordinate = new GPSCoordinate(43.651070, -79.347015);
            }
            else
            {
                coordinate.Latitude = Input.location.lastData.latitude;
                coordinate.Longitude = Input.location.lastData.longitude;
            }

            using UnityWebRequest www = UnityWebRequest.Get($"https://api.open-meteo.com/v1/forecast?latitude={coordinate.Latitude}&longitude={coordinate.Longitude}&hourly=weather_code,temperature_2m,precipitation,precipitation_probability&current=temperature_2m,weather_code,cloud_cover,precipitation&timezone=auto");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
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

        public IEnumerator GetTourProximity()
        {
            Task<TourDTO> task = IsTourInProximityAsync();

            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.Result != null)
            {
                popupManager.ShowTourProximityPopup(task.Result);
            }
        }

        public async Task<TourDTO> IsTourInProximityAsync(GPSCoordinate coord = null)
        {
            var tours = await DatabaseController.GetAllTours();
            foreach (TourDTO tourDTO in tours)
            {
                var distance = GPSCoordinate.CalculateGPSDistance(
                        new GPSCoordinate(tourDTO.StartLatitude, tourDTO.StartLongitude),
                        coord ?? gps
                    );

                if (distance < TOUR_PROXIMITY_DISTANCE_METERS)
                {
                    Debug.Log($"Tour {tourDTO.Name} found {Math.Round(distance, 3)}m away");
                    return tourDTO;
                }
            }
            return null;
        }

        public static GPSCoordinate GetCurrentGPSCoordinates()
        {
            return gps.Clone();
        }

        private void StopGPS()
        {
            Input.location.Stop();
        }

        public int CheckForWeatherHazards()
        {
            if (HAZARD_WEATHER_CODES.TryGetValue(weather.Current.WeatherCode, out string codeMsg))
            {
                Debug.Log("WEATHER HAZARD: " + codeMsg);
                popupManager.ShowWeatherHazardPopup(codeMsg);
                return weather.Current.WeatherCode;
            }
            return -1;
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
            return $"({Math.Round(Latitude, 4)}, {Math.Round(Longitude, 4)})";
        }

        public string ToString(int dec)
        {
            return $"({Math.Round(Latitude, dec)}, {Math.Round(Longitude, dec)})";
        }

        public static double CalculateGPSDistance(GPSCoordinate coord1, GPSCoordinate coord2)
        {
            double lat1Rad = DegreesToRadians(coord1.Latitude);
            double lon1Rad = DegreesToRadians(coord1.Longitude);
            double lat2Rad = DegreesToRadians(coord2.Latitude);
            double lon2Rad = DegreesToRadians(coord2.Longitude);

            double deltaLat = lat2Rad - lat1Rad;
            double deltaLon = lon2Rad - lon1Rad;

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                    Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                    Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return c * 6371000;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
