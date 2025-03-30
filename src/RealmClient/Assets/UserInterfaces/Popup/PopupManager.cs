using UnityEngine.UIElements;
using UnityEngine;

namespace Realm.Popup
{
    public class PopupManager : MonoBehaviour
    {
        [SerializeField]
        private UIDocument popupUI;

        public void ShowWeatherHazardPopup(string weatherCodeMsg)
        {
            PopupCustomControl popup = new(true);
            popup.SetText($"Weather Hazard:\n {weatherCodeMsg}");
            popup.SetPrimaryButtonText("OK");

            popupUI.rootVisualElement.Add(popup);

            popup.Primary += () => popupUI.rootVisualElement.Remove(popup);
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
