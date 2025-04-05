using UnityEngine;

public class Marker : MonoBehaviour
{

    public Vector3 position;
    public Vector3 realWorldCoords;
    public string mname;
    public bool isVisible;
    int a;

    public GameObject markerPrefab;
    public GameObject markerObject;

    public Marker(GameObject markerPrefab, string name, float longitude, float latitude, float altitude, bool visible = false)
    {
        mname = name;
        position = new Vector3(longitude, latitude, altitude);
        isVisible = visible;
        markerObject = Instantiate(markerPrefab);
        setVisibility(isVisible);
    }

    public void setVisibility(bool visible)
    {
        isVisible = visible;
        if (markerObject != null)
        {
            markerObject.SetActive(isVisible);
        }
    }

    public void setPosition(Vector3 newPos)
    {
        position = new Vector3(newPos.x, newPos.y, newPos.z);
        if (markerObject != null)
        {
            markerObject.transform.position = position;
        }
    }

}
