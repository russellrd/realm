using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace Realm.Controller
{
    public class SwitchController : MonoBehaviour
    {
        [SerializeField]
        private NavigationDisplay navigationDisplay;

        [SerializeField]
        private PlacementController placementController;

        [SerializeField]
        private GameObject objectSpawner;

        [SerializeField]
        private Button openUIButton;

        [SerializeField]
        private ARPlaneManager planeManager;

        [SerializeField]
        private GameObject HUD;

        [SerializeField]
        private XROrigin xrOrigin;

        public static NavigationController.Destinations uiScreen;

        public static Dictionary<string, string> data;

        public static bool editMode = false;

        public static bool switched = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            openUIButton.onClick.AddListener(SwitchToUI);

        }

        // Update is called once per frame
        void Update()
        {
            if (switched)
            {
                SetMode();
                switched = false;
            }
        }

        public void SwitchToUI()
        {
            navigationDisplay.Show(uiScreen, data);
            Clear();
        }

        public void Clear()
        {
            uiScreen = NavigationController.Destinations.none;
            data = null;
        }

        public void SetMode()
        {
            List<GameObject> objectSpawnerChildren = new();
            GameObjectUtils.GetChildGameObjects(objectSpawner, objectSpawnerChildren);

            if (!editMode)
                placementController.CancelPlacement();

            var trackablesTransform = xrOrigin.gameObject.transform.Find("Trackables");
            trackablesTransform?.gameObject.SetActive(editMode);

            HUD.SetActive(editMode);

            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(editMode);
            }
            planeManager.enabled = editMode;

            // foreach (var o in objectSpawnerChildren)
            // {
            //     o.GetComponent<XRGrabInteractable>().enabled = true;
            // }
        }
    }
}
