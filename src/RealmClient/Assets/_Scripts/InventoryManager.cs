using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Realm
{
    public class InventoryManager : MonoBehaviour
    {
        public static readonly int MAX_OBJ_COUNT = 15;

        [SerializeField]
        private UIInventory uiInventory;

        [SerializeField]
        private GameObject HUD;

        [SerializeField]
        private Image previewerImage;

        [SerializeField]
        private ObjectSpawner objectSpawner;

        // List<InventoryARObjectPreview> initialARObjectPreviews = new();

        public ModelStore modelStore;

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
        }

        void OnDisable()
        {
            m_CreateButton.onClick.RemoveListener(OpenInventory);
        }

        async void Awake()
        {
            while (!modelStore.initialized)
            {
                await Task.Delay(1000);
            }
            Debug.Log($"INVENTORY MANAGER START (1)");
            Debug.Log($"INVENTORY MANAGER START - keys contains e0a1khy0514cahw?: {modelStore.sprites.ContainsKey("e0a1khy0514cahw")}");
            // foreach (string modelId in modelStore.getKeys())
            // {
            //     ARObjectPreviewSO aRObjectPreviewSO = new();
            //     ModelDTO modelData = null;
            //     Sprite sprite = null;
            //     modelStore.modelData.TryGetValue(modelId, out modelData);
            //     modelStore.sprites.TryGetValue(modelId, out sprite);
            //     Debug.Log($"INVENTORY MANAGER AWAKE model: {modelId}, sprite: {sprite != null}");
            //     aRObjectPreviewSO.Name = modelData.Name;
            //     aRObjectPreviewSO.Author = modelData.Creator;
            //     aRObjectPreviewSO.Description = modelData.Name;
            //     aRObjectPreviewSO.PreviewImage = sprite;

            //     initialARObjectPreviews.Add(
            //         new InventoryARObjectPreview
            //         {
            //             arObjectPreview = aRObjectPreviewSO
            //         }
            //     );
            // }
        }

        void Start()
        {
            CloseInventory();
            uiInventory.InitInventory();
            uiInventory.OnSelected += HandleDescriptionRequest;
        }

        private void HandleDescriptionRequest(int index)
        {
            SetObjectToSpawn(index);
            // InventoryARObjectPreview inventoryARObjectPreview = inventoryData.GetARObjectPreviewAt(index);
            // if (inventoryARObjectPreview.IsEmpty)
            // {
            //     uiInventory.DeselectAllARObjectPreviews();
            //     return;
            // }
            // ARObjectPreviewSO arObjectPreview = inventoryARObjectPreview.arObjectPreview;
            previewerImage.sprite = uiInventory.GetPreviewSpriteAtIndex(index);
            uiInventory.Select(index);
        }

        public void OpenInventory()
        {
            isInventoryOpen = true;
            uiInventory.Show();
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
            modelStore.modelObjects.TryGetValue(model, out GameObject gameObject);
            return gameObject;
        }
    }
}
