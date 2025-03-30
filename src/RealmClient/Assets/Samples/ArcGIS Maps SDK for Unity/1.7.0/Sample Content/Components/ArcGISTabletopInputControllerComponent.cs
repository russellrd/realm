// COPYRIGHT 1995-2023 ESRI
// TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
// Unpublished material - all rights reserved under the
// Copyright Laws of the United States and applicable international
// laws, treaties, and conventions.
//
// For additional information, contact:
// Attn: Contracts and Legal Department
// Environmental Systems Research Institute, Inc.
// 380 New York Street
// Redlands, California 92373
// USA
//
// email: legal@esri.com
using Esri.ArcGISMapsSDK.Components;
using Esri.HPFramework;
using Unity.Mathematics;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && USE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
#endif

namespace Esri.ArcGISMapsSDK.Samples.Components
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Tools/ArcGIS Maps SDK/Samples/ArcGIS Tabletop Input Controller")]
	public class ArcGISTabletopInputControllerComponent : MonoBehaviour
	{
		public ArcGISTabletopControllerComponent tabletopControllerComponent;

		private Vector3 dragStartPoint = Vector3.zero;
		private double4x4 dragStartWorldMatrix;
		private bool isDragging = false;
		private bool isZooming = false;
		private HPRoot hpRoot;
		private ArcGISMapComponent mapComponent;
#if ENABLE_INPUT_SYSTEM && USE_INPUT_SYSTEM
		private float inputScalar = 120.0f;
#endif
		private float zoomStartDistance = 0;
		private const double zoomScalar = 20;

		public void EndPointDrag()
		{
			isDragging = false;
		}

		public void EndTwoPointZoom()
		{
			isZooming = false;
		}

#if ENABLE_INPUT_SYSTEM && USE_INPUT_SYSTEM
		private void OnDisable()
		{
			EnhancedTouchSupport.Disable();
		}
#endif

		private void OnEnable()
		{
			hpRoot = FindFirstObjectByType<HPRoot>();
			mapComponent = FindFirstObjectByType<ArcGISMapComponent>();
#if ENABLE_INPUT_SYSTEM && USE_INPUT_SYSTEM
			// Touch Input
			EnhancedTouchSupport.Enable();
#endif
		}

		public void StartPointDrag(Vector2 screenPoint)
		{
			Vector3 dragCurrentPoint;
			var dragStartRay = Camera.main.ScreenPointToRay(screenPoint);

			if (tabletopControllerComponent.Raycast(dragStartRay, out dragCurrentPoint))
			{
				isDragging = true;
				dragStartPoint = dragCurrentPoint;

				// Save the matrix to go from Local space to Universe space
				// As the origin location will be changing during drag, we keep the transform we had when the action started
				dragStartWorldMatrix = math.mul(math.inverse(hpRoot.WorldMatrix), tabletopControllerComponent.transform.localToWorldMatrix.ToDouble4x4());
			}
		}

		public void StartTwoPointZoom(Vector3 position0, Vector3 position1)
		{
			Vector3 zoomCurrentPoint0;
			Vector3 zoomCurrentPoint1;
			var zoomStartRay0 = Camera.main.ScreenPointToRay(position0);
			var zoomStartRay1 = Camera.main.ScreenPointToRay(position1);

			if (tabletopControllerComponent.Raycast(zoomStartRay0, out zoomCurrentPoint0) && tabletopControllerComponent.Raycast(zoomStartRay1, out zoomCurrentPoint1))
			{
				isZooming = true;

				zoomStartDistance = Vector3.Distance(zoomCurrentPoint0, zoomCurrentPoint1);
			}
		}

		private void Update()
		{
#if ENABLE_INPUT_SYSTEM && USE_INPUT_SYSTEM
			if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count < 1)
			{
				if (Mouse.current.leftButton.IsPressed() && !isDragging)
				{
					StartPointDrag(Mouse.current.position.ReadValue());
				}
				else if (Mouse.current.leftButton.IsPressed() && isDragging)
				{
					UpdatePointDrag(Mouse.current.position.ReadValue());
				}
				else if (!Mouse.current.leftButton.IsPressed() && isDragging)
				{
					EndPointDrag();
				}

				if (Mouse.current.scroll.ReadValue().y != 0.0f)
				{
					ZoomMap((Mouse.current.scroll.ReadValue().y / inputScalar), Mouse.current.position.ReadValue());
				}
			}
			else if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count == 1)
			{
				var touch0 = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0];

				if (touch0.phase == UnityEngine.InputSystem.TouchPhase.Began)
				{
					StartPointDrag(touch0.screenPosition);
				}
				else if (touch0.phase == UnityEngine.InputSystem.TouchPhase.Moved)
				{
					UpdatePointDrag(touch0.screenPosition);
				}
				else
				{
					EndPointDrag();
				}
			}
			else if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count == 2)
			{
				var touch0 = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0];
				var touch1 = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[1];

				if (touch0.phase == UnityEngine.InputSystem.TouchPhase.Began || touch1.phase == UnityEngine.InputSystem.TouchPhase.Began)
				{
					StartTwoPointZoom(touch0.screenPosition, touch1.screenPosition);
				}
				else if (touch0.phase == UnityEngine.InputSystem.TouchPhase.Moved || touch1.phase == UnityEngine.InputSystem.TouchPhase.Moved)
				{
					UpdateTwoPointZoom(touch0.screenPosition, touch1.screenPosition);
				}
				else if (touch0.phase == UnityEngine.InputSystem.TouchPhase.Ended || touch1.phase == UnityEngine.InputSystem.TouchPhase.Ended)
				{
					EndTwoPointZoom();
				}
			}
#else
			// Handle mouse inputs.
			if (Input.touchCount < 2)
			{
				if (Input.GetMouseButtonDown(0))
				{
					StartPointDrag(Input.mousePosition);
				}
				else if (Input.GetMouseButton(0))
				{
					UpdatePointDrag(Input.mousePosition);
				}
				else if (Input.GetMouseButtonUp(0))
				{
					EndPointDrag();
				}

				if (Input.mouseScrollDelta.y != 0.0)
				{
					var zoom = Mathf.Sign(Input.mouseScrollDelta.y);
					ZoomMap(zoom, Input.mousePosition);
				}
			}
			// Handle two-finger inputs.
			else if (Input.touchCount == 2)
			{
				var touch0 = Input.GetTouch(0);
				var touch1 = Input.GetTouch(1);

				if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
				{
					StartTwoPointZoom(touch0.position, touch1.position);
				}
				else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
				{
					UpdateTwoPointZoom(touch0.position, touch1.position);
				}
				else if (touch0.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Ended)
				{
					EndTwoPointZoom();
				}
			}
#endif

		}

		public void UpdatePointDrag(Vector3 screenPoint)
		{
			if (isDragging)
			{
				var updateRay = Camera.main.ScreenPointToRay(screenPoint);

				Vector3 dragCurrentPoint;
				tabletopControllerComponent.Raycast(updateRay, out dragCurrentPoint);

				var diff = dragStartPoint - dragCurrentPoint;
				var newExtentCenterCartesian = dragStartWorldMatrix.HomogeneousTransformPoint(diff.ToDouble3());
				var newExtentCenterGeographic = mapComponent.View.WorldToGeographic(new double3(newExtentCenterCartesian.x, newExtentCenterCartesian.y, newExtentCenterCartesian.z));

				tabletopControllerComponent.Center = newExtentCenterGeographic;
			}
		}

		public void UpdateTwoPointZoom(Vector3 position0, Vector3 position1)
		{
			if (isZooming)
			{
				Vector3 zoomCurrentPoint0;
				Vector3 zoomCurrentPoint1;
				var zoomRay0 = Camera.main.ScreenPointToRay(position0);
				var zoomRay1 = Camera.main.ScreenPointToRay(position1);

				if (tabletopControllerComponent.Raycast(zoomRay0, out zoomCurrentPoint0) && tabletopControllerComponent.Raycast(zoomRay1, out zoomCurrentPoint1))
				{
					var zoomCurrentDistance = Vector3.Distance(zoomCurrentPoint0, zoomCurrentPoint1);
					var diff = zoomCurrentDistance - zoomStartDistance;

					// More zoom means smaller extent
					tabletopControllerComponent.Width -= diff * tabletopControllerComponent.Width / 10;
					if (tabletopControllerComponent.Shape == MapExtentShapes.Rectangle)
					{
						tabletopControllerComponent.Height -= diff * tabletopControllerComponent.Height / 10;
					}
				}
			}
		}

		public void ZoomMap(float zoom, Vector3 screenPoint)
		{
			if (zoom == 0)
			{
				return;
			}

			Vector3 outPoint;
			var zoomRay = Camera.main.ScreenPointToRay(screenPoint);

			if (tabletopControllerComponent.Raycast(zoomRay, out outPoint))
			{
				// More zoom means smaller extent
				tabletopControllerComponent.Width -= zoom * tabletopControllerComponent.Width / zoomScalar;
				if (tabletopControllerComponent.Shape == MapExtentShapes.Rectangle)
				{
					tabletopControllerComponent.Height -= zoom * tabletopControllerComponent.Height / zoomScalar;
				}
			}
		}
	}
}
