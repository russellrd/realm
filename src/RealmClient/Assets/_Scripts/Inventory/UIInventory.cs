using System;
using System.Collections.Generic;
using Realm;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField]
    private UIInventoryARObjectPreview arObjectPreviewPrefab;

    [SerializeField]
    private RectTransform content;

    List<UIInventoryARObjectPreview> arObjectPreviewPrefabList = new();

    private readonly List<Sprite> previewSprites = new();

    public event Action<int> OnSelected;

    public void InitInventory()
    {
        previewSprites.Add(Resources.Load<Sprite>("Sprites/ARObjectPreview/bowl_of_freshly_cut_fruit"));
        previewSprites.Add(Resources.Load<Sprite>("Sprites/ARObjectPreview/coffee_mug_with_pattern"));
        previewSprites.Add(Resources.Load<Sprite>("Sprites/ARObjectPreview/blue_ceramic_plate_with_pizza"));
        previewSprites.Add(Resources.Load<Sprite>("Sprites/ARObjectPreview/classic_red_telephone"));
        previewSprites.Add(Resources.Load<Sprite>("Sprites/ARObjectPreview/colorful_crayons"));
        previewSprites.Add(Resources.Load<Sprite>("Sprites/ARObjectPreview/classic_wristwatch_leather_strap"));
        previewSprites.Add(Resources.Load<Sprite>("Sprites/ARObjectPreview/colorful_balloons"));
        previewSprites.Add(Resources.Load<Sprite>("Sprites/ARObjectPreview/bicycle_with_basket"));
        previewSprites.Add(Resources.Load<Sprite>("Sprites/ARObjectPreview/black_umbrella"));

        var invSize = previewSprites.Count > InventoryManager.MAX_OBJ_COUNT ? InventoryManager.MAX_OBJ_COUNT : previewSprites.Count;
        Debug.Log(invSize);
        for (int i = 0; i < invSize; i++)
        {
            UIInventoryARObjectPreview arObjectPreview = Instantiate(arObjectPreviewPrefab, Vector3.zero, Quaternion.identity);
            arObjectPreview.transform.SetParent(content);
            arObjectPreview.transform.localScale = new Vector3(1, 1, 1);
            arObjectPreviewPrefabList.Add(arObjectPreview);
            UpdateData(i, previewSprites[i]);
            arObjectPreview.OnARObjectPreviewClicked += HandleARObjectPreviewSelection;
        }
    }

    public void UpdateData(int index, Sprite image)
    {
        if (arObjectPreviewPrefabList.Count > index)
        {
            arObjectPreviewPrefabList[index].SetData(image);
        }
    }

    public void HandleARObjectPreviewSelection(UIInventoryARObjectPreview arObjectPreview)
    {
        int index = arObjectPreviewPrefabList.IndexOf(arObjectPreview);
        if (index == -1)
            return;
        OnSelected?.Invoke(index);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void DeselectAllARObjectPreviews()
    {
        foreach (UIInventoryARObjectPreview arObjectPreview in arObjectPreviewPrefabList)
        {
            arObjectPreview.Deselect();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Select(int index)
    {
        DeselectAllARObjectPreviews();
        arObjectPreviewPrefabList[index].Select();
    }

    public Sprite GetPreviewSpriteAtIndex(int index)
    {
        return arObjectPreviewPrefabList[index].GetData();
    }

    internal void ResetAllARObjectPreviews()
    {
        foreach (var arObjectPreview in arObjectPreviewPrefabList)
        {
            arObjectPreview.Reset();
            arObjectPreview.Deselect();
        }
    }
}
