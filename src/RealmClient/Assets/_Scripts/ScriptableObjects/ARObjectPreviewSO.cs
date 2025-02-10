using UnityEngine;

[CreateAssetMenu(fileName = "AR Object Preview", menuName = "Scriptable Objects/AR Object Preview")]
public class ARObjectPreviewSO : ScriptableObject
{
    public int ID => GetInstanceID();

    [field: SerializeField]
    public string Name { get; set; }

    [field: SerializeField]
    public string Author { get; set; }

    [field: SerializeField]
    public string Description { get; set; }

    [field: SerializeField]
    public Sprite PreviewImage { get; set; }

    [field: SerializeField]
    public GameObject Model { get; set; }
}
