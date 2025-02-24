using System;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField]
    private UIInventoryARObjectPreview arObjectPreviewPrefab;

    [SerializeField]
    private RectTransform content;

    List<UIInventoryARObjectPreview> arObjectPreviewPrefabList = new();

    public event Action<int> OnDescriptionRequested;

    public void InitInventory(int size)
    {
        for (int i = 0; i < size; i++)
        {
            UIInventoryARObjectPreview arObjectPreview = Instantiate(arObjectPreviewPrefab, Vector3.zero, Quaternion.identity);
            arObjectPreview.transform.SetParent(content);
            arObjectPreview.transform.localScale = new Vector3(1, 1, 1);
            arObjectPreviewPrefabList.Add(arObjectPreview);
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
        OnDescriptionRequested?.Invoke(index);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        DeselectAllARObjectPreviews();
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

    internal void UpdateDescription(int index, Sprite previewImage, string name, string description)
    {
        // TODO: Implement description later
        DeselectAllARObjectPreviews();
        arObjectPreviewPrefabList[index].Select();
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
