using UnityEngine;
using UnityEngine.AI;

namespace Realm
{
    public class TextObject : MonoBehaviour
    {
        private Vector3 originalRotation;

        public GameObject border;

        public Camera mainCamera;

        void Awake()
        {
            originalRotation = transform.rotation.eulerAngles;
        }

        void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = mainCamera.ScreenPointToRay(touch.position);
                    RaycastHit raycastHit;
                    if (Physics.Raycast(ray, out raycastHit))
                    {
                        Debug.Log(raycastHit.transform.gameObject);
                        if (raycastHit.transform.gameObject == border)
                        {
                            Debug.Log("HIT!");
                        }
                    }
                }
            }
        }

        // void LateUpdate()
        // {
        //     transform.forward = Camera.main.transform.forward;
        //     Vector3 rotation = transform.rotation.eulerAngles;
        //     rotation.x = 0;
        //     transform.rotation = Quaternion.Euler(rotation);
        // }
    }
}
