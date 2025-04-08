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

    private List<Sprite> previewSprites;

    public event Action<int> OnSelected;

    public void InitInventory()
    {
        previewSprites = new List<Sprite>
        {
            Resources.Load<Sprite>("Sprites/ARObjectPreview/fruit_bowl"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/umbrella"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/tree"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/coffee_mug"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/lamp"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/balloons"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/rocket"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/house"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/crayons"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/pizza"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/guitar"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/ring"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/classic_phone"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/backpack"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/bicycle"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/plane"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/wristwatch"),
            Resources.Load<Sprite>("Sprites/ARObjectPreview/arrow_sign"),
        };

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
