using UnityEngine;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using System.Collections.Generic;
using Esri.HPFramework;
using Unity.Mathematics;

public class TourMarkerManager : MonoBehaviour
{
    const float altitudeOffset = -175f;
    // const float altitudeOffset = 5f;
    public GameObject tourMarkerPrefab;
    private List<Marker> TourMarkers = new List<Marker>();
    private ArcGISMapComponent arcGISMap;
    private HPRoot mapHPRoot;

    void Start()
    {
        arcGISMap = FindFirstObjectByType<ArcGISMapComponent>();
        mapHPRoot = arcGISMap.GetComponent<HPRoot>();
    }

    public void AddMarker(float longitude, float latitude, float altitude, string name = "", bool visible = false)
    {
        Vector3 realWorldPos = ConvertToUnityCoords(longitude, latitude, altitude);
        Marker mark = new Marker(tourMarkerPrefab, name, realWorldPos.x, realWorldPos.y, realWorldPos.z);
        mark.markerObject.transform.SetParent(mapHPRoot.transform, false);
        mark.realWorldCoords = new Vector3(longitude, latitude, altitude);
        mark.setVisibility(visible);
        TourMarkers.Add(mark);
        Debug.Log($"Created Tour Marker: {mark.mname} at {mark.position}");
    }

    public bool RemoveMarker(Marker marker)
    {
        if (marker != null)
        {
            Debug.Log($"Destroyed Tour Marker{marker.mname} at {marker.position}");
            TourMarkers.Remove(marker);
            Destroy(marker);
            return true;
        }
        return false;
    }

    public Marker GetMarker(int instanceID)
    {
        Marker mark = null;
        foreach (Marker m in TourMarkers)
        {
            if (m.markerObject.GetInstanceID() == instanceID)
            {
                mark = m;
            }
        }
        return mark;
    }

    public int markerCount()
    {
        return TourMarkers.Count;
    }

    public Marker GetMarker(string markName)
    {
        Marker mark = null;
        foreach (Marker m in TourMarkers)
        {
            if (m.mname == markName)
            {
                mark = m;
            }
        }
        return mark;
    }

    public List<Marker> GetMarkerList()
    {
        return TourMarkers;
    }

    public void UpdateMarkerPos(Marker marker, float longitude, float latitude, float altitude)
    {
        Vector3 realWorldPos = ConvertToUnityCoords(longitude, latitude, altitude);
        marker.setPosition(realWorldPos);
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

}
