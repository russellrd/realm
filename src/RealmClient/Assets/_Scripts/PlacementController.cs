using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.XR.ARCoreExtensions;
using PocketBaseSdk;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

namespace Realm
{
    public class PlacementController : MonoBehaviour
    {
        [SerializeField]
        InventoryManager InvManager;

        [SerializeField]
        ObjectSpawner objectSpawner;

        [SerializeField]
        TMP_Text helperText;

        [SerializeField]
        GameObject CloudAnchorPrefab;

        [SerializeField]
        GameObject MapQualityIndicatorPrefab;

        [SerializeField]
        [Tooltip("Button that deletes a selected object.")]
        Button m_DeleteButton;

        public Button deleteButton
        {
            get => m_DeleteButton;
            set => m_DeleteButton = value;
        }

        [SerializeField]
        [Tooltip("Button that confirms a placement.")]
        Button m_ConfirmButton;

        public Button confirmButton
        {
            get => m_ConfirmButton;
            set => m_ConfirmButton = value;
        }

        [SerializeField]
        [Tooltip("Button that cancels a placement.")]
        Button m_CancelButton;

        public Button cancelButton
        {
            get => m_CancelButton;
            set => m_CancelButton = value;
        }

        [SerializeField]
        [Tooltip("The interaction for the AR scene.")]
        XRInteractionGroup m_Interaction;

        public XRInteractionGroup interaction
        {
            get => m_Interaction;
            set => m_Interaction = value;
        }

        [SerializeField]
        XRInputValueReader<Vector2> m_TapStartPositionInput = new XRInputValueReader<Vector2>("Tap Start Position");

        public XRInputValueReader<Vector2> tapStartPositionInput
        {
            get => m_TapStartPositionInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_TapStartPositionInput, value, this);
        }

        [SerializeField]
        XRInputValueReader<Vector2> m_DragCurrentPositionInput = new XRInputValueReader<Vector2>("Drag Current Position");

        public XRInputValueReader<Vector2> dragCurrentPositionInput
        {
            get => m_DragCurrentPositionInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_DragCurrentPositionInput, value, this);
        }

        public XROrigin Origin;

        public ARSession SessionCore;

        public ARCoreExtensions Extensions;

        public ARAnchorManager AnchorManager;

        public ARPlaneManager PlaneManager;

        public ARRaycastManager RaycastManager;

        [HideInInspector]
        public PlacementStep Step = PlacementStep.None;

        public HashSet<string> ResolvingSet = new HashSet<string>();

        public enum PlacementStep
        {
            None,
            Inventory,
            Place,
            Interact,
            Anchor
        }

        bool m_IsPointerOverUI;

        private GPSCoordinate anchorCoordinates = null;

        private MapQualityIndicator _qualityIndicator = null;

        private ARAnchor _anchor = null;

        private HostCloudAnchorPromise _hostPromise = null;

        private HostCloudAnchorResult _hostResult = null;

        private IEnumerator _hostCoroutine = null;

        private List<ResolveCloudAnchorPromise> _resolvePromises =
            new List<ResolveCloudAnchorPromise>();

        private List<ResolveCloudAnchorResult> _resolveResults =
            new List<ResolveCloudAnchorResult>();

        private List<IEnumerator> _resolveCoroutines = new List<IEnumerator>();

        void OnEnable()
        {
            m_DeleteButton.onClick.AddListener(DeleteSelectedObject);
            m_CancelButton.onClick.AddListener(CancelPlacement);
            m_ConfirmButton.onClick.AddListener(ConfirmPlacement);

            _anchor = null;
            _hostPromise = null;
            _hostResult = null;
            _hostCoroutine = null;
            _resolvePromises.Clear();
            _resolveResults.Clear();
            _resolveCoroutines.Clear();

            m_ConfirmButton.gameObject.SetActive(false);
            m_CancelButton.gameObject.SetActive(false);
        }

        void OnDisable()
        {
            m_DeleteButton.onClick.RemoveListener(DeleteSelectedObject);
            m_CancelButton.onClick.RemoveListener(CancelPlacement);
            m_ConfirmButton.onClick.RemoveListener(ConfirmPlacement);

            if (_anchor != null)
            {
                Destroy(_anchor.gameObject);
                _anchor = null;
            }

            if (_hostCoroutine != null)
            {
                StopCoroutine(_hostCoroutine);
            }

            _hostCoroutine = null;

            if (_hostPromise != null)
            {
                _hostPromise.Cancel();
                _hostPromise = null;
            }

            _hostResult = null;

            foreach (var coroutine in _resolveCoroutines)
            {
                StopCoroutine(coroutine);
            }

            _resolveCoroutines.Clear();

            foreach (var promise in _resolvePromises)
            {
                promise.Cancel();
            }

            _resolvePromises.Clear();

            foreach (var result in _resolveResults)
            {
                if (result.Anchor != null)
                {
                    Destroy(result.Anchor.gameObject);
                }
            }

            _resolveResults.Clear();
        }

        void Start()
        {
            Invoke("LoadAnchors", 1.5f);
        }

        private async void LoadAnchors()
        {
            List<AnchorDTO> anchors = await DatabaseController.GetAllARObjects();
            foreach (AnchorDTO anchorDTO in anchors)
            {
                ResolvingSet.Add(anchorDTO.AnchorId);
            }
        }

        public void Update()
        {
            m_IsPointerOverUI = false;
            switch (Step)
            {
                case PlacementStep.None:
                    if (InvManager.IsInventoryOpen())
                    {
                        if (!m_IsPointerOverUI && (m_TapStartPositionInput.TryReadValue(out _) || m_DragCurrentPositionInput.TryReadValue(out _)))
                        {

                            if (InvManager.IsInventoryOpen())
                                InvManager.CloseInventory();
                        }

                        m_IsPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1);
                    }
                    else
                    {
                        _ = ResolvingCloudAnchors();
                    }

                    m_Interaction.gameObject.SetActive(true);
                    m_CancelButton.gameObject.SetActive(false);
                    m_ConfirmButton.gameObject.SetActive(false);
                    InvManager.createButton.gameObject.SetActive(true);
                    m_DeleteButton.gameObject.SetActive(m_Interaction?.focusInteractable != null);
                    helperText.gameObject.SetActive(false);
                    objectSpawner.GetComponent<ARInteractorSpawnTriggerNew>().enabled = true;
                    break;
                case PlacementStep.Interact:
                    m_ConfirmButton.gameObject.SetActive(true);
                    m_CancelButton.gameObject.SetActive(true);
                    m_DeleteButton.gameObject.SetActive(false);
                    break;
                case PlacementStep.Anchor:
                    if (_anchor == null)
                    {
                        Vector2 pos;
                        // Debug.Log(m_IsPointerOverUI);
                        // Debug.Log(m_TapStartPositionInput.TryReadValue(out _));
                        // Debug.Log(m_DragCurrentPositionInput.TryReadValue(out _));
                        if (m_TapStartPositionInput.TryReadValue(out pos))
                        {
                            PerformHitTest(pos);
                        }
                        m_IsPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1);
                    }

                    helperText.gameObject.SetActive(true);
                    m_Interaction.gameObject.SetActive(false);
                    m_ConfirmButton.gameObject.SetActive(false);
                    InvManager.createButton.gameObject.SetActive(false);

                    HostingCloudAnchor();
                    break;
            }
        }

        private void PerformHitTest(Vector2 touchPos)
        {
            List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
            RaycastManager.Raycast(touchPos, hitResults, TrackableType.PlaneWithinPolygon);

            // If there was an anchor placed, then instantiate the corresponding object.
            var planeType = PlaneAlignment.HorizontalUp;
            if (hitResults.Count > 0)
            {
                ARPlane plane = PlaneManager.GetPlane(hitResults[0].trackableId);
                if (plane == null)
                {
                    Debug.LogWarningFormat("Failed to find the ARPlane with TrackableId {0}",
                        hitResults[0].trackableId);
                    return;
                }

                planeType = plane.alignment;
                var hitPose = hitResults[0].pose;
                // if (Application.platform == RuntimePlatform.IPhonePlayer)
                // {
                //     // Point the hitPose rotation roughly away from the raycast/camera
                //     // to match ARCore.
                //     hitPose.rotation.eulerAngles = new Vector3(0.0f, controller.MainCamera.transform.eulerAngles.y, 0.0f);
                // }

                _anchor = AnchorManager.AttachAnchor(plane, hitPose);
            }

            if (_anchor != null)
            {
                GetPlacementObject().SetActive(false);
                Instantiate(CloudAnchorPrefab, _anchor.transform);

                // Attach map quality indicator to this anchor.
                var indicatorGO = Instantiate(MapQualityIndicatorPrefab, _anchor.transform);
                _qualityIndicator = indicatorGO.GetComponent<MapQualityIndicator>();
                _qualityIndicator.DrawIndicator(planeType, MainCamera);

                helperText.text = " To save this location, walk around the object to " +
                    "capture it from different angles";
                // DebugText.text = "Waiting for sufficient mapping quaility...";

                // Hide plane generator so users can focus on the object they placed.
                UpdatePlaneVisibility(false);
            }
        }

        private void HostingCloudAnchor()
        {
            Debug.Log("HostingCloudAnchor");
            // There is no anchor for hosting.
            if (_anchor == null)
            {
                return;
            }

            Debug.Log("1");

            // There is a pending or finished hosting task.
            if (_hostPromise != null || _hostResult != null)
            {
                return;
            }
            Debug.Log("2");

            // Update map quality:
            int qualityState = 2;
            // Can pass in ANY valid camera pose to the mapping quality API.
            // Ideally, the pose should represent users’ expected perspectives.
            FeatureMapQuality quality =
                AnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());
            helperText.text = "Current mapping quality: " + quality;
            qualityState = (int)quality;
            _qualityIndicator.UpdateQualityState(qualityState);

            // Hosting instructions:
            var cameraDist = (_qualityIndicator.transform.position -
                MainCamera.transform.position).magnitude;
            if (cameraDist < _qualityIndicator.Radius * 1.5f)
            {
                helperText.text = "You are too close, move backward.";
                return;
            }
            else if (cameraDist > 10.0f)
            {
                helperText.text = "You are too far, come closer.";
                return;
            }
            else if (_qualityIndicator.ReachTopviewAngle)
            {
                helperText.text =
                    "You are looking from the top view, move around from all sides.";
                return;
            }
            else if (!_qualityIndicator.ReachQualityThreshold)
            {
                helperText.text = "Save the object here by capturing it from all sides.";
                return;
            }

            Debug.Log("3");
            helperText.text = "Mapping quality has reached sufficient threshold, " +
                "creating Cloud Anchor.";
            helperText.text = string.Format(
                "FeatureMapQuality has reached {0}, triggering CreateCloudAnchor.",
                AnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose()));

            // Creating a Cloud Anchor with lifetime = 1 day.
            // This is configurable up to 365 days when keyless authentication is used.
            Debug.Log("CreateAnchor");
            var promise = AnchorManager.HostCloudAnchorAsync(_anchor, 1);
            if (promise.State == PromiseState.Done)
            {
                Debug.LogFormat("Failed to host a Cloud Anchor.");
                OnAnchorHostedFinished(false);
            }
            else
            {
                _hostPromise = promise;
                _hostCoroutine = HostAnchor();
                StartCoroutine(_hostCoroutine);
            }
        }

        private IEnumerator HostAnchor()
        {
            Debug.Log("HOSTANCHOR");
            yield return _hostPromise;
            _hostResult = _hostPromise.Result;
            _hostPromise = null;

            if (_hostResult.CloudAnchorState == CloudAnchorState.Success)
            {
                Debug.Log("SUCCESS");
                // int count = controller.LoadCloudAnchorHistory().Collection.Count;
                // _hostedCloudAnchor =
                //     new CloudAnchorHistory("CloudAnchor" + count, _hostResult.CloudAnchorId);
                OnAnchorHostedFinished(true, _hostResult.CloudAnchorId);
            }
            else
            {
                Debug.Log("FAILURE");
                OnAnchorHostedFinished(false, _hostResult.CloudAnchorState.ToString());
            }
        }

        private async Task ResolvingCloudAnchors()
        {
            // No Cloud Anchor for resolving.
            if (ResolvingSet.Count == 0)
            {
                return;
            }

            // There are pending or finished resolving tasks.
            if (_resolvePromises.Count > 0 || _resolveResults.Count > 0)
            {
                return;
            }

            // ARCore session is not ready for resolving.
            if (ARSession.state != ARSessionState.SessionTracking)
            {
                return;
            }

            Debug.LogFormat("Attempting to resolve {0} Cloud Anchor(s): {1}",
                ResolvingSet.Count,
                string.Join(",", new List<string>(ResolvingSet).ToArray()));
            foreach (string cloudId in ResolvingSet)
            {
                var promise = AnchorManager.ResolveCloudAnchorAsync(cloudId);
                if (promise.State == PromiseState.Done)
                {
                    Debug.LogFormat("Failed to resolve Cloud Anchor " + cloudId);
                    OnAnchorResolvedFinished(false, cloudId);
                }
                else
                {
                    var arObject = await DatabaseController.GetARObjectFromAnchorId(cloudId);
                    _resolvePromises.Add(promise);
                    var coroutine = ResolveAnchor(arObject, promise);
                    StartCoroutine(coroutine);
                }
            }

            ResolvingSet.Clear();
        }

        private IEnumerator ResolveAnchor(AnchorDTO arObject, ResolveCloudAnchorPromise promise)
        {
            yield return promise;
            var result = promise.Result;
            _resolvePromises.Remove(promise);
            _resolveResults.Add(result);

            if (result.CloudAnchorState == CloudAnchorState.Success)
            {
                OnAnchorResolvedFinished(true, arObject.AnchorId);
                result.Anchor.transform.localScale = new Vector3(arObject.Scale, arObject.Scale, arObject.Scale);
                Debug.Log(arObject.ModelId);
                Debug.Log("X: " + result.Anchor.transform.localScale.x);
                Debug.Log("Y: " + result.Anchor.transform.localScale.y);
                Debug.Log("Z: " + result.Anchor.transform.localScale.z);
                GameObject go = Instantiate(InvManager.GetPrefab(arObject.ModelId), result.Anchor.transform.position, result.Anchor.transform.rotation);
                go.GetComponent<ARTransformer>().enabled = false;
            }
            else
            {
                OnAnchorResolvedFinished(false, arObject.AnchorId, result.CloudAnchorState.ToString());
            }
        }

        private void OnAnchorHostedFinished(bool success, string response = null)
        {
            if (success)
            {
                helperText.text = "Host success!";
                UpdatePlaneVisibility(true);
                GetPlacementObject().SetActive(true);
                GameObject anchor = GameObject.FindGameObjectWithTag("anchor");

                GameObject go = Instantiate(InvManager.GetPrefab(GetPlacementObjectID()), anchor.transform.position, anchor.transform.rotation);
                go.GetComponent<ARTransformer>().enabled = false;

                saveARObject(
                    "test",
                    response,
                    DatabaseController.pb.AuthStore.Model.Id,
                    GetPlacementObject().name,
                    GetPlacementObject().transform.localScale.x,
                    anchorCoordinates.Latitude,
                    anchorCoordinates.Longitude
                );
                Destroy(anchor);
                Destroy(GetPlacementObject().gameObject);
                Step = PlacementStep.None;
            }
            else
            {
                helperText.text = "Host failed.";
                UpdatePlaneVisibility(true);
                CancelPlacement();
            }
        }

        private void OnAnchorResolvedFinished(bool success, string cloudId, string response = null)
        {
            if (success)
            {
                helperText.text = "Resolve success!";
            }
            else
            {
                helperText.text = "Resolve failed.";
            }
        }

        public void CancelPlacement()
        {
            if (objectSpawner.gameObject.transform.childCount > 0)
            {
                Destroy(GetPlacementObject());
            }
            Step = PlacementStep.None;
            _anchor = null;
            UpdatePlaneVisibility(true);
            Destroy(GameObject.FindGameObjectWithTag("anchor"));
            Destroy(GameObject.FindGameObjectWithTag("indicator"));
        }

        private void UpdatePlaneVisibility(bool visible)
        {
            foreach (var plane in PlaneManager.trackables)
            {
                plane.gameObject.SetActive(visible);
            }
        }

        public GameObject GetPlacementObject()
        {
            return objectSpawner.gameObject.transform.GetChild(0).gameObject;
        }

        public string GetPlacementObjectID()
        {
            return objectSpawner.selectedObjectId;
        }

        public void ConfirmPlacement()
        {
            anchorCoordinates = RealWorldController.GetCurrentGPSCoordinates();
            Step = PlacementStep.Anchor;
        }

        void DeleteSelectedObject()
        {
            var currentSelectedObject = m_Interaction.focusInteractable;
            if (currentSelectedObject != null)
            {
                // TODO: Remove from DB if allowed
                Destroy(currentSelectedObject.transform.gameObject);
            }
        }

        public Camera MainCamera
        {
            get
            {
                return Origin.Camera;
            }
        }

        public Pose GetCameraPose()
        {
            return new Pose(MainCamera.transform.position, MainCamera.transform.rotation);
        }

        public async void saveARObject(string name, string anchorId, string userId, string modelId, float scale, double latitude, double longitude)
        {
            AnchorDTO anchorDTO = new()
            {
                Name = name,
                AnchorId = anchorId,
                UserId = userId,
                ModelId = modelId,
                Scale = scale,
                Latitude = latitude,
                Longitude = longitude
            };

            try
            {
                var anchor = await DatabaseController.pb.Collection("ar_objects").Create(anchorDTO);
            }
            catch (Exception e)
            {
                print(e.InnerException.ToString());
            }
        }
    }
}
