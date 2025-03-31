using UnityEngine;
using System.Collections.Generic;

namespace Realm
{
    public class CreateMarkerManager : MonoBehaviour
    {
        // List<TourDTO> tours = new List<TourDTO>();
        TourMarkerManager tourMarkerManager;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public async void Start()
        {
            List<TourDTO> tours = await DatabaseController.GetAllTours();
            tourMarkerManager = FindFirstObjectByType<TourMarkerManager>();
            if (tourMarkerManager != null)
            {
                foreach (TourDTO tour in tours)
                {
                    Debug.Log($"Tour Name: {tour.Name}, Description: {tour.Description}");
                    // Add markers for each tour
                    tourMarkerManager.AddMarker((float)tour.StartLongitude, (float)tour.StartLatitude, 0, tour.Name, true);
                }
            }
            else
            {
                Debug.LogError("TourMarkerManager not found in scene");
            }

        }
    }
}
