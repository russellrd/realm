using UnityEngine;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using Unity.Mathematics;
using System.Collections;

public class UserMarkerManager : MonoBehaviour
{
    const float altitudeOffset = -175f;
    // const float altitudeOffset = 10f;
    const float camAltitude = 1000f;
    private Marker userMarker;
    private ArcGISMapComponent arcGISMap;
    private HPRoot mapHPRoot;
    UserLocationManager userLocationManager;

    [Header("Marker Prefab")]
    public GameObject userMarkerPrefab;

    [Header("Camera Recenter Settings")]
    public ArcGISCameraComponent arcGISCamera;
    public Vector3 cameraOffset = new Vector3(0, camAltitude - altitudeOffset, 0);
    public float recenterDuration = 3.0f;  // Duration for smooth movement.

    void Start()
    {
        arcGISMap = FindFirstObjectByType<ArcGISMapComponent>();
        mapHPRoot = arcGISMap.GetComponent<HPRoot>();
        userLocationManager = FindFirstObjectByType<UserLocationManager>();
        if (userLocationManager == null)
        {
            Debug.LogError("UserLocationManager not found in the scene!");
        }
        CreateUserMarker();
    }

    void Update()
    {
        UpdateUserPosition();
    }

    private void CreateUserMarker()
    {
        Vector3 userPos = new Vector3(userLocationManager.longitude, userLocationManager.latitude, userLocationManager.altitude);
        userMarker = new Marker(userMarkerPrefab, "User", userPos.x, userPos.y, userPos.z);
        userMarker.markerObject.transform.SetParent(mapHPRoot.transform, false);
        userMarker.realWorldCoords = userPos;
        userMarker.setVisibility(true);
        Debug.Log($"Created User Marker: {userMarker.mname} at {userMarker.position}");
    }

    public void ToggleUserVisibility()
    {
        if (userMarker != null)
        {
            userMarker.setVisibility(!userMarker.isVisible);
        }
        userMarker.isVisible = !userMarker.isVisible;
    }

    // Convert real-world geographic coordinates to unity coordinates
    private Vector3 ConvertToUnityCoords(float longitude, float latitude, float altitude)
    {
        ArcGISPoint geoPos = new ArcGISPoint(longitude, latitude, altitude, ArcGISSpatialReference.WGS84());
        double3 unityWorldPos = arcGISMap.View.GeographicToWorld(geoPos);
        double3 correctedPos = mapHPRoot.TransformPoint(unityWorldPos);
        correctedPos.y = altitudeOffset; // Offset marker height from map

        return correctedPos.ToVector3();
    }

    private void UpdateUserPosition()
    {
        userMarker.setPosition(ConvertToUnityCoords(userLocationManager.longitude, userLocationManager.latitude, userLocationManager.altitude));
    }

}
