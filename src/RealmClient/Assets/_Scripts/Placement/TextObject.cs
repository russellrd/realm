using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Realm
{
    public class TextObject : MonoBehaviour
    {
        private Vector3 originalRotation;

        public GameObject textObjectCollider;

        public TextMeshPro textMeshProObject;

        public GameObject textObjectUI;

        public Button saveButton;

        public TMP_InputField inputField;

        public Camera arCamera;

        public ARRaycastManager arRaycastManager;

        private List<ARRaycastHit> hits = new List<ARRaycastHit>();

        private TMP_Text boardText;

        void Awake()
        {
            originalRotation = transform.rotation.eulerAngles;
        }

        void OnEnable()
        {
            saveButton.onClick.AddListener(Save);
            boardText = textMeshProObject.GetComponent<TMP_Text>();
            inputField.onValueChanged.AddListener(UpdateBoard);
            // TODO: Get from DB and if not default text
            inputField.text = "HELLO THERE";
        }

        void Update()
        {
            if (textObjectUI.activeSelf)
                return;

            // if (Input.touchCount > 0)
            // {
            //     Touch touch = Input.GetTouch(0);

            //     if (touch.phase == TouchPhase.Began)
            //     {
            //         Debug.Log("touch");
            //         Ray ray = arCamera.ScreenPointToRay(touch.position);
            //         if (Physics.Raycast(ray, out RaycastHit hitObject))
            //         {
            //             Debug.Log(hitObject.transform.gameObject);
            //             if (hitObject.transform.gameObject == textObjectCollider)
            //             {
            //                 Debug.Log("HIT!");
            //             }
            //         }
            //     }
            // }

            if (Input.GetMouseButton(0))
            {
                Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitObject))
                {
                    Debug.Log(hitObject.transform.gameObject);
                }

                if (arRaycastManager.Raycast(ray, hits, TrackableType.AllTypes))
                {
                    if (hitObject.transform.gameObject == textObjectCollider)
                    {
                        textObjectUI.SetActive(true);
                    }
                }
            }

        }

        void Save()
        {
            // Upload
            Debug.Log(inputField.text);
            textObjectUI.SetActive(false);
        }

        void UpdateBoard(string data)
        {
            boardText.text = data;
        }

        void LateUpdate()
        {
            // transform.forward = arCamera.transform.forward;

            Vector3 newPosition = transform.position;
            newPosition.y = arCamera.transform.position.y;
            transform.position = newPosition;

            // Vector3 rotation = transform.rotation.eulerAngles;
            // rotation.x = 0;
            // transform.rotation = Quaternion.Euler(rotation);
        }
    }
}
