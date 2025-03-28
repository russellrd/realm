using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
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

    bool isInventoryOpen;

    [SerializeField]
    [Tooltip("Button that opens the inventory.")]
    Button m_CreateButton;

    public Button createButton
    {
        get => m_CreateButton;
        set => m_CreateButton = value;
    }

    void OnEnable()
    {
        isInventoryOpen = false;
        m_CreateButton.onClick.AddListener(OpenInventory);

        modelsToPrefab.Add("Plant", objectSpawner.objectPrefabs[0]);
        modelsToPrefab.Add("Book", objectSpawner.objectPrefabs[1]);
        modelsToPrefab.Add("Bench", objectSpawner.objectPrefabs[2]);
        modelsToPrefab.Add("Hammer", objectSpawner.objectPrefabs[3]);
        modelsToPrefab.Add("Lamp", objectSpawner.objectPrefabs[4]);
    }

    void OnDisable()
    {
        m_CreateButton.onClick.RemoveListener(OpenInventory);
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

    public void OpenInventory()
    {
        isInventoryOpen = true;
        uiInventory.Show();
        foreach (var arObjectPreview in inventoryData.GetCurrentInventoryState())
        {
            uiInventory.UpdateData(arObjectPreview.Key, arObjectPreview.Value.arObjectPreview.PreviewImage);
        }
        HUD.SetActive(false);
    }

    public void CloseInventory()
    {
        isInventoryOpen = false;
        uiInventory.Hide();
        HUD.SetActive(true);
    }

    public bool IsInventoryOpen()
    {
        return isInventoryOpen;
    }

    public void SetObjectToSpawn(int index)
    {
        if (objectSpawner.objectPrefabs.Count > index)
        {
            objectSpawner.spawnOptionIndex = index;
        }
    }

    public GameObject GetPrefab(string model)
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