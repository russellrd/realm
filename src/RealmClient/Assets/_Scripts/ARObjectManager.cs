using UnityEngine;

public class ARObjectManager : MonoBehaviour
{
    private string _associatedAnchorId;
    public string AssociatedAnchorId
    {
        get => _associatedAnchorId;
        set => _associatedAnchorId = value;
    }
}
