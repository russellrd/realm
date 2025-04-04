using UnityEngine;
using UnityEngine.Android;
using System.Collections;

public class UserLocationManager : MonoBehaviour
{
    private bool isTracking = false;
    public float latitude;
    public float longitude;
    public float altitude;

    void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Debug.Log("Requesting User Location...");
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        if (Input.location.isEnabledByUser)
        {
            StartCoroutine(GetUserLocation());
        }
        else
        {
            Debug.LogError("GPS is not enabled on this device.");
        }
    }

    private IEnumerator GetUserLocation()
    {
        if (!Input.location.isEnabledByUser)
        {
#if UNITY_ANDROID
            Permission.RequestUserPermission(Permission.FineLocation);
#endif
            Debug.LogError("GPS is disabled! Enable it in device location settings.");
            yield break;
        }

        Input.location.Start(); // Start GPS tracking

        int maxWait = 5;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location.");
            yield break;
        }

        isTracking = true;
        UpdateUserPosition();
    }

    private void Update()
    {
        if (isTracking)
        {
            UpdateUserPosition();
        }
    }

    private void UpdateUserPosition()
    {
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        altitude = Input.location.lastData.altitude;
    }

}
