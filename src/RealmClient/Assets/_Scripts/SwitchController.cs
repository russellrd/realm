using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Realm.Controller
{
    public class SwitchController : MonoBehaviour
    {
        [SerializeField]
        private NavigationManager navigationManager;

        [SerializeField]
        private Button openUIButton;

        private bool uiOpen = true;

        public static NavigationController.Destinations uiScreen;

        public static Dictionary<string, string> data;

        public static bool editMode;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            openUIButton.onClick.AddListener(SwitchToUI);
        }

        // Update is called once per frame
        void Update()
        {
            // if (uiScreen != NavigationController.Destinations.none)
            // {
            //     navigationManager.gameObject.SetActive(false);
            // }
        }

        public void SwitchToUI()
        {
            Debug.Log("SwitchToUI");
            Debug.Log(uiScreen);
            Debug.Log(data);
            // navigationManager.gameObject.SetActive(true);
            navigationManager.Show(uiScreen, data);
            Clear();
        }

        public void Clear()
        {
            uiScreen = NavigationController.Destinations.none;
            data = null;
        }
    }
}
