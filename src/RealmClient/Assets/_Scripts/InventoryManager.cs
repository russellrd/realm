using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;
using System.Collections;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using Unity.VisualScripting;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    PlacementController controller;

    [SerializeField]
    DatabaseController DBCon;

    [SerializeField]
    GPSController GPSCon;

    [SerializeField]
    GameObject CloudAnchorPrefab;

    [SerializeField]
    GameObject MapQualityIndicatorPrefab;

    [SerializeField]
    [Tooltip("Button that opens the create menu.")]
    Button m_CreateButton;

    public Button createButton
    {
        get => m_CreateButton;
        set => m_CreateButton = value;
    }

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

    [SerializeField]
    private TMP_Text helperText;

    [SerializeField]
    private UIInventory uiInventory;

    [SerializeField]
    private InventorySO inventoryData;

    [SerializeField]
    private GameObject HUD;

    [SerializeField]
    private Image previewerImage;

    [SerializeField]
    private ObjectSpawner objectSpawner;

    public List<InventoryARObjectPreview> initialARObjectPreviews = new();

    public Dictionary<string, GameObject> modelsToPrefab;

    bool m_IsPointerOverUI;
    bool m_ShowObjectInventory;

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

    public Pose GetCameraPose()
    {
        return new Pose(controller.MainCamera.transform.position,
            controller.MainCamera.transform.rotation);
    }

    void OnEnable()
    {
        m_ShowObjectInventory = false;
        m_CreateButton.onClick.AddListener(OpenInventory);
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

        modelsToPrefab.Add("Plant", objectSpawner.objectPrefabs[0]);
        modelsToPrefab.Add("Book", objectSpawner.objectPrefabs[1]);
        modelsToPrefab.Add("Bench", objectSpawner.objectPrefabs[2]);
        modelsToPrefab.Add("Hammer", objectSpawner.objectPrefabs[3]);
        modelsToPrefab.Add("Lamp", objectSpawner.objectPrefabs[4]);
    }

    void OnDisable()
    {
        m_CreateButton.onClick.RemoveListener(OpenInventory);
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
        CloseInventory();
        uiInventory.InitInventory(inventoryData.Size);
        uiInventory.OnDescriptionRequested += HandleDescriptionRequest;

        inventoryData.Initialize();
        inventoryData.OnInventoryUpdated += UpdateInventoryUI;
        foreach (InventoryARObjectPreview arObjectPreview in initialARObjectPreviews)
        {
            if (arObjectPreview.IsEmpty)
                continue;
            inventoryData.AddARObjectPreview(arObjectPreview);
        }

        Invoke("LoadAnchors", 1.5f);
    }

    private async void LoadAnchors()
    {
        List<AnchorDTO> anchors = await controller.getAllARObjects();
        foreach (AnchorDTO anchorDTO in anchors)
        {
            controller.ResolvingSet.Add(anchorDTO.AnchorId);
        }
    }

    private void UpdateInventoryUI(Dictionary<int, InventoryARObjectPreview> inventoryState)
    {
        uiInventory.ResetAllARObjectPreviews();
        foreach (var arObjectPreview in inventoryState)
        {
            uiInventory.UpdateData(arObjectPreview.Key, arObjectPreview.Value.arObjectPreview.PreviewImage);
        }
    }


    private void HandleDescriptionRequest(int index)
    {
        SetObjectToSpawn(index);
        InventoryARObjectPreview inventoryARObjectPreview = inventoryData.GetARObjectPreviewAt(index);
        if (inventoryARObjectPreview.IsEmpty)
        {
            uiInventory.DeselectAllARObjectPreviews();
            return;
        }
        ARObjectPreviewSO arObjectPreview = inventoryARObjectPreview.arObjectPreview;
        previewerImage.sprite = arObjectPreview.PreviewImage;
        uiInventory.UpdateDescription(index, arObjectPreview.PreviewImage, arObjectPreview.Name, arObjectPreview.Description);
    }

    public void Update()
    {
        m_IsPointerOverUI = false;
        switch (controller.Step)
        {
            case PlacementController.PlacementStep.None:
                if (m_ShowObjectInventory)
                {
                    if (!m_IsPointerOverUI && (m_TapStartPositionInput.TryReadValue(out _) || m_DragCurrentPositionInput.TryReadValue(out _)))
                    {

                        if (m_ShowObjectInventory)
                            CloseInventory();
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
                m_CreateButton.gameObject.SetActive(true);
                m_DeleteButton.gameObject.SetActive(m_Interaction?.focusInteractable != null);
                helperText.gameObject.SetActive(false);
                objectSpawner.GetComponent<ARInteractorSpawnTriggerNew>().enabled = true;
                break;
            case PlacementController.PlacementStep.Interact:
                // objectSpawner.gameObject.SetActive(false);
                m_ConfirmButton.gameObject.SetActive(true);
                m_CancelButton.gameObject.SetActive(true);
                m_DeleteButton.gameObject.SetActive(false);
                break;
            case PlacementController.PlacementStep.Anchor:
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
                m_CreateButton.gameObject.SetActive(false);

                HostingCloudAnchor();
                break;
        }
    }

    private void PerformHitTest(Vector2 touchPos)
    {
        List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
        controller.RaycastManager.Raycast(touchPos, hitResults, TrackableType.PlaneWithinPolygon);

        // If there was an anchor placed, then instantiate the corresponding object.
        var planeType = PlaneAlignment.HorizontalUp;
        if (hitResults.Count > 0)
        {
            ARPlane plane = controller.PlaneManager.GetPlane(hitResults[0].trackableId);
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

            _anchor = controller.AnchorManager.AttachAnchor(plane, hitPose);
        }

        if (_anchor != null)
        {
            getPlacementObject().SetActive(false);
            Instantiate(CloudAnchorPrefab, _anchor.transform);

            // Attach map quality indicator to this anchor.
            var indicatorGO = Instantiate(MapQualityIndicatorPrefab, _anchor.transform);
            _qualityIndicator = indicatorGO.GetComponent<MapQualityIndicator>();
            _qualityIndicator.DrawIndicator(planeType, controller.MainCamera);

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
            controller.AnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());
        helperText.text = "Current mapping quality: " + quality;
        qualityState = (int)quality;
        _qualityIndicator.UpdateQualityState(qualityState);

        // Hosting instructions:
        var cameraDist = (_qualityIndicator.transform.position -
            controller.MainCamera.transform.position).magnitude;
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
            controller.AnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose()));

        // Creating a Cloud Anchor with lifetime = 1 day.
        // This is configurable up to 365 days when keyless authentication is used.
        Debug.Log("CreateAnchor");
        var promise = controller.AnchorManager.HostCloudAnchorAsync(_anchor, 1);
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
        if (controller.ResolvingSet.Count == 0)
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
            controller.ResolvingSet.Count,
            string.Join(",", new List<string>(controller.ResolvingSet).ToArray()));
        foreach (string cloudId in controller.ResolvingSet)
        {
            var promise = controller.AnchorManager.ResolveCloudAnchorAsync(cloudId);
            if (promise.State == PromiseState.Done)
            {
                Debug.LogFormat("Failed to resolve Cloud Anchor " + cloudId);
                OnAnchorResolvedFinished(false, cloudId);
            }
            else
            {
                var arObject = await controller.getARObjectFromAnchorId(cloudId);
                _resolvePromises.Add(promise);
                var coroutine = ResolveAnchor(arObject, promise);
                StartCoroutine(coroutine);
            }
        }

        controller.ResolvingSet.Clear();
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
            GameObject go = getPrefab(arObject.ModelId);
            go.GetComponent<ARTransformer>().enabled = false;
            Instantiate(getPrefab(arObject.ModelId), result.Anchor.transform.position, result.Anchor.transform.rotation);
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
            getPlacementObject().SetActive(true);
            GameObject anchor = GameObject.FindGameObjectWithTag("anchor");
            GameObject go = getPrefab(getPlacementObject().gameObject.name);
            go.GetComponent<ARTransformer>().enabled = false;
            Instantiate(go, anchor.transform.position, anchor.transform.rotation);
            controller.saveARObject(
                "test",
                response,
                DBCon.pb.AuthStore.Model.Id,
                getPlacementObject().name,
                getPlacementObject().transform.localScale.x,
                anchorCoordinates.latitude,
                anchorCoordinates.longitude
            );
            Destroy(anchor);
            Destroy(getPlacementObject().gameObject);
            controller.Step = PlacementController.PlacementStep.None;
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

    private void UpdatePlaneVisibility(bool visible)
    {
        foreach (var plane in controller.PlaneManager.trackables)
        {
            plane.gameObject.SetActive(visible);
        }
    }

    public void OpenInventory()
    {
        m_ShowObjectInventory = true;
        uiInventory.Show();
        foreach (var arObjectPreview in inventoryData.GetCurrentInventoryState())
        {
            uiInventory.UpdateData(arObjectPreview.Key, arObjectPreview.Value.arObjectPreview.PreviewImage);
        }
        HUD.SetActive(false);
    }

    public void CloseInventory()
    {
        m_ShowObjectInventory = false;
        uiInventory.Hide();
        HUD.SetActive(true);
    }

    public void CancelPlacement()
    {
        if (objectSpawner.gameObject.transform.childCount > 0)
        {
            Destroy(getPlacementObject());
        }
        controller.Step = PlacementController.PlacementStep.None;
        _anchor = null;
        UpdatePlaneVisibility(true);
        Destroy(GameObject.FindGameObjectWithTag("anchor"));
        Destroy(GameObject.FindGameObjectWithTag("indicator"));
    }

    public GameObject getPlacementObject()
    {
        return objectSpawner.gameObject.transform.GetChild(0).gameObject;
    }

    public void ConfirmPlacement()
    {
        anchorCoordinates = GPSCon.GetCurrentGPSCoordinates();
        controller.Step = PlacementController.PlacementStep.Anchor;
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

    public void SetObjectToSpawn(int index)
    {
        if (objectSpawner.objectPrefabs.Count > index)
        {
            objectSpawner.spawnOptionIndex = index;
        }
    }

    private GameObject getPrefab(string model)
    {
        switch (model)
        {
            case "Plant(Clone)":
                return objectSpawner.objectPrefabs[0];
            case "Book(Clone)":
                return objectSpawner.objectPrefabs[1];
            case "Bench(Clone)":
                return objectSpawner.objectPrefabs[2];
            case "Hammer(Clone)":
                return objectSpawner.objectPrefabs[3];
            case "Lamp(Clone)":
                return objectSpawner.objectPrefabs[4];
            default:
                return objectSpawner.objectPrefabs[0];
        }
    }
}