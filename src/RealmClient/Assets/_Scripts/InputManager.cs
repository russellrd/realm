using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class InputManager : MonoBehaviour
{
    public GameObject arObject;
    public Camera arCamera;

    public ARRaycastManager _raycaseManager;

    List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);
            if (_raycaseManager.Raycast(ray, _hits))
            {
                Pose pose = _hits[0].pose;
                Instantiate(arObject, pose.position, pose.rotation);
            }
        }
    }
}
