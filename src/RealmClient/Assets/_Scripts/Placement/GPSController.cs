using System.Collections;
using UnityEngine;

public class GPSController : MonoBehaviour
{
    private const char UNITS = 'K';
    private const int MAX_WAIT_TIME = 15;

    // public GPSCoordinate currentCoordinate = new GPSCoordinate();

    [HideInInspector]
    public bool gpsOn = false;

    IEnumerator Start()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("Location not enabled");
            // TODO: Add popup msg
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
            // TODO: Add popup
            yield break;
        }

        gpsOn = true;

        Debug.Log("INITIAL GPS: " + Input.location.lastData.longitude + " " + Input.location.lastData.latitude);
    }

    void Update()
    {
        if (!gpsOn)
        {
            // TODO: Add popup msg
        }
    }

    public GPSCoordinate GetCurrentGPSCoordinates()
    {
        return new GPSCoordinate(Input.location.lastData.latitude, Input.location.lastData.longitude);
    }

    public void StopGPS()
    {
        Input.location.Stop();
    }
}

public class GPSCoordinate
{
    public double latitude { get; set; }
    public double longitude { get; set; }

    public GPSCoordinate()
    {
        latitude = 0;
        longitude = 0;
    }

    public GPSCoordinate(double latitude, double longitude)
    {
        this.latitude = latitude;
        this.longitude = longitude;
    }

    public override string ToString()
    {
        return "(" + latitude + ", " + longitude + ")";
    }
}