using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Objects/Inventory")]
public class InventorySO : ScriptableObject
{
    [SerializeField]
    private List<InventoryARObjectPreview> inventoryARObjectPreviews;

    [field: SerializeField]
    public int Size { get; set; } = 10;

    public event Action<Dictionary<int, InventoryARObjectPreview>> OnInventoryUpdated;

    public void Initialize()
    {
        inventoryARObjectPreviews = new();
        for (int i = 0; i < Size; i++)
        {
            inventoryARObjectPreviews.Add(InventoryARObjectPreview.GetEmptyARObjectPreview());
        }
    }

    public void AddARObjectPreview(ARObjectPreviewSO arObjectPreview)
    {
        for (int i = 0; i < inventoryARObjectPreviews.Count; i++)
        {
            if (inventoryARObjectPreviews[i].IsEmpty)
            {
                inventoryARObjectPreviews[i] = new InventoryARObjectPreview
                {
                    arObjectPreview = arObjectPreview
                };
                return;
            }
        }
    }

    public void AddARObjectPreview(InventoryARObjectPreview arObjectPreview)
    {
        AddARObjectPreview(arObjectPreview.arObjectPreview);
    }

    public Dictionary<int, InventoryARObjectPreview> GetCurrentInventoryState()
    {
        Dictionary<int, InventoryARObjectPreview> returnValue = new();
        for (int i = 0; i < inventoryARObjectPreviews.Count; i++)
        {
            if (inventoryARObjectPreviews[i].IsEmpty)
                continue;
            returnValue[i] = inventoryARObjectPreviews[i];
        }
        return returnValue;
    }

    public InventoryARObjectPreview GetARObjectPreviewAt(int index)
    {
        return inventoryARObjectPreviews[index];
    }
}

[Serializable]
public struct InventoryARObjectPreview
{
    public ARObjectPreviewSO arObjectPreview;
    public readonly bool IsEmpty => arObjectPreview == null;

    public static InventoryARObjectPreview GetEmptyARObjectPreview() => new()
    {
        arObjectPreview = null
    };
}