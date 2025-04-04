using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryARObjectPreview : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Image previewImage;

    [SerializeField]
    private Image borderImage;

    public event Action<UIInventoryARObjectPreview> OnARObjectPreviewClicked;

    void Awake()
    {
        Reset();
        Deselect();
    }

    public void Reset()
    {
        previewImage.gameObject.SetActive(false);
    }

    public void Deselect()
    {
        borderImage.enabled = false;
    }

    public void SetData(Sprite sprite)
    {
        Debug.Log("DATA SET" + sprite.name);
        previewImage.gameObject.SetActive(true);
        previewImage.sprite = sprite;
    }

    public Sprite GetData()
    {
        return previewImage.sprite;
    }

    public void Select()
    {
        borderImage.enabled = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnARObjectPreviewClicked?.Invoke(this);
    }
}
