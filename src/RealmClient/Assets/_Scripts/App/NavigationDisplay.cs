using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using Realm.Controller;

namespace Realm
{
    public class NavigationDisplay : MonoBehaviour
    {
        [SerializeField]
        private UIDocument uiDocument;

        public static VisualElement main;

        public static VisualElement container;

        public static VisualElement panel;

        void Start()
        {
            main = uiDocument.rootVisualElement;

            Show(NavigationController.Destinations.splash, null);
        }

        public void Show(NavigationController.Destinations dest, Dictionary<string, string> data)
        {
            Debug.Log("Show" + dest.ToString());
            container = new VisualElement();
            container.AddToClassList("container");
            container.StretchToParentSize();
            main.Add(container);

            panel = new VisualElement();
            panel.AddToClassList("sub-container");
            panel.StretchToParentSize();
            container.Add(panel);

            NavigationController.NavigateTo(dest, data);
        }

        public static void SwitchToRealm(NavigationController.Destinations uiScreen, bool editMode, Dictionary<string, string> data = null)
        {
            Debug.Log("SwitchToRealm");
            SwitchController.uiScreen = uiScreen;
            SwitchController.data = data;
            SwitchController.editMode = editMode;
            NavigationController.ClearScreen();
            main.Remove(container);
            main.Clear();
        }
    }
}
