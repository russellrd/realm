using UnityEngine;
using UnityEngine.UI;

public class ARObjectPreviewManager : MonoBehaviour
{
    private Button btn;

    private GameObject arObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(SelectARObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SelectARObject()
    {
    }
}
