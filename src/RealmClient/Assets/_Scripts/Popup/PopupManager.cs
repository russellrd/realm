using UnityEngine.UIElements;
using UnityEngine;
using Realm.Popup;
using System.Collections.Generic;

namespace Realm
{
    public class PopupManager : MonoBehaviour
    {
        [SerializeField]
        private UIDocument popupUI;

        [SerializeField]
        private NavigationDisplay navigationDisplay;

        public void ShowWeatherHazardPopup(string weatherCodeMsg)
        {
            PopupCustomControl popup = new(true);
            popup.SetText($"Weather Hazard:\n {weatherCodeMsg}");
            popup.SetPrimaryButtonText("OK");

            popupUI.rootVisualElement.Add(popup);

            popup.Primary += () => popupUI.rootVisualElement.Remove(popup);
        }

        public void ShowTourProximityPopup(TourDTO tour)
        {
            PopupCustomControl popup = new(false, true);
            popup.SetText($"{tour.Name}\n{tour.Description}\nPreview Tour?");
            popup.SetPrimaryButtonText("NO");
            popup.SetSecondaryButtonText("YES");

            popupUI.rootVisualElement.Add(popup);

            popup.Primary += () => popupUI.rootVisualElement.Remove(popup);
            popup.Secondary += () => navigationDisplay.Show(
                NavigationController.Destinations.tour_preview,
                new Dictionary<string, string> { { "tourId", tour.Id } }
            );
        }

        public void ShowErrorPopup(string errorMsg)
        {
            PopupCustomControl popup = new(true);
            popup.SetText($"Error:\n {errorMsg}");
            popup.SetPrimaryButtonText("CLOSE");

            popupUI.rootVisualElement.Add(popup);

            popup.Primary += () => popupUI.rootVisualElement.Remove(popup);
        }
    }
}
