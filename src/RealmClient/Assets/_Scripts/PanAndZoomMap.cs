using System;
using UnityEngine;

public class PanAndZoomMap : MonoBehaviour
{
    private Vector2 lastTouchPos;
    private bool isPanning;
    private bool isZooming;
    private bool isRotating = false;
    private Camera mainCamera;

    public float panThreshold = 1f;
    public float zoomThreshold = 1f;

    private float zoomSpeed = 23f;
    private float minHeight = 300;
    private float maxHeight = 5000000;

    private float lastAngle = 0;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 1) // Panning
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPos = touch.position;
                isPanning = true;
            }
            else if (touch.phase == TouchPhase.Moved && isPanning)
            {
                if (!isPanning && Vector2.Distance(lastTouchPos, touch.position) < panThreshold)
                {
                    isPanning = false;
                }
                if (isPanning)
                {
                    Vector2 delta = touch.deltaPosition;
                    PanMap(delta);
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isPanning = true;
            }
        }
        else if (Input.touchCount == 2) // Pinch Zoom & Rotation
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            Vector2 touch1Prev = touch1.position - touch1.deltaPosition;
            Vector2 touch2Prev = touch2.position - touch2.deltaPosition;

            float prevDistance = Vector2.Distance(touch1Prev, touch2Prev);
            float currentDistance = Vector2.Distance(touch1.position, touch2.position);

            float zoomDelta = (prevDistance - currentDistance) * 0.9f;
            if (Math.Abs(zoomDelta) > zoomThreshold)
            {
                isZooming = true;
            }

            if (isZooming)
            {
                // Compute midpoint of the pinch
                Vector2 pinchMidPoint = (touch1.position + touch2.position) / 2;
                ZoomMap(zoomDelta, pinchMidPoint);
            }

            // Rotation
            float prevAngle = Mathf.Atan2(touch1Prev.y - touch2Prev.y, touch1Prev.x - touch2Prev.x) * Mathf.Rad2Deg;
            float currentAngle = Mathf.Atan2(touch1.position.y - touch2.position.y, touch1.position.x - touch2.position.x) * Mathf.Rad2Deg;

            float angleDelta = currentAngle - prevAngle;

            if (!isRotating)
            {
                lastAngle = currentAngle;
                isRotating = true;
            }

            float rotationSensitivity = 1f;
            float rotationThreshold = 1f;

            if (Mathf.Abs(angleDelta) > rotationThreshold)
            {
                transform.Rotate(Vector3.up, angleDelta * rotationSensitivity, Space.World);
            }
            lastAngle = currentAngle;
        }
        else
        {
            isPanning = false;
            isZooming = false;
            isRotating = false;
        }
    }

    private void PanMap(Vector2 delta)
    {
        // // float dynamicMoveSpeed = moveSpeed * (transform.position.y / maxHeight);
        // float dynamicMoveSpeed = moveSpeed * Mathf.Log(transform.position.y + 1);

        float baseMoveSpeed = 3.0f;
        // float dynamicMoveSpeed = baseMoveSpeed * Mathf.Clamp(transform.position.y / minHeight, 1f, 15f);
        float dynamicMoveSpeed = baseMoveSpeed * Mathf.Lerp(2f, 120f, Mathf.InverseLerp(minHeight, maxHeight, transform.position.y));

        transform.position += new Vector3(-delta.x * dynamicMoveSpeed, 0, -delta.y * dynamicMoveSpeed);


    }

    private void ZoomMap(float delta, Vector2 pinchMidPoint)
    {
        // float dynamicZoomSpeed = zoomSpeed * Mathf.Log(transform.position.y + 1);
        float dynamicZoomSpeed = zoomSpeed * Mathf.Lerp(2f, 50f, Mathf.InverseLerp(minHeight, maxHeight, transform.position.y));

        Vector3 newPosition = transform.position - transform.forward * delta * dynamicZoomSpeed;
        newPosition.y = Mathf.Clamp(newPosition.y, minHeight, maxHeight);

        Ray ray = mainCamera.ScreenPointToRay(pinchMidPoint);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 targetPoint = ray.GetPoint(enter);

            Vector3 direction = targetPoint - transform.position;
            transform.position += direction * delta * dynamicZoomSpeed;
        }

        transform.position = newPosition;
    }

}
