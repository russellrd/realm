// COPYRIGHT 1995-2024 ESRI
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
using Esri.ArcGISMapsSDK.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esri.ArcGISMapsSDK.Samples.Components
{
	[DisallowMultipleComponent]
	[ExecuteAlways]
	public class ArcGISAttributionDisplay : MonoBehaviour
	{
		private ArcGISMapComponent mapComponent;

		private VisualElement background;
		private Label label;

		private bool expanded = false;

		private void OnEnable()
		{
			var rootElement = GetComponent<UIDocument>().rootVisualElement;

			if (rootElement == null)
			{
				return;
			}

			background = rootElement.Q<VisualElement>("background");
			background.RegisterCallback<PointerDownEvent>(BackgroundClicked);

			label = rootElement.Q<Label>("attributionLabel");
			expanded = false;
			SetLabelExpanded();

			mapComponent = FindFirstObjectByType<ArcGISMapComponent>();

			if (!mapComponent || !mapComponent.View)
			{
				Debug.LogError("Unable to find a parent ArcGISMapComponent.");

				enabled = false;
				return;
			}

			SetAttributionText(mapComponent.View.AttributionText);

			mapComponent.View.AttributionChanged += () =>
			{
				SetAttributionText(mapComponent.View.AttributionText);

				expanded = false;
				SetLabelExpanded();
			};
		}

		private void OnDisable()
		{
			if (mapComponent && mapComponent.View)
			{
				mapComponent.View.AttributionChanged = null;
			}

			background?.UnregisterCallback<PointerDownEvent>(BackgroundClicked);

			SetAttributionText(string.Empty);
		}

		private void BackgroundClicked(PointerDownEvent evt)
		{
			evt.StopImmediatePropagation();
			expanded = !expanded;
			SetLabelExpanded();
		}

		private void SetAttributionText(string text)
		{
			if (label == null)
			{
				return;
			}

			ArcGISMainThreadScheduler.Instance().Schedule(() =>
			{
				label.text = text;
			});
		}

		private void SetLabelExpanded()
		{
			ArcGISMainThreadScheduler.Instance().Schedule(() =>
			{
				label.style.overflow = expanded ? Overflow.Visible : Overflow.Hidden;
				label.style.textOverflow = expanded ? TextOverflow.Clip : TextOverflow.Ellipsis;
				label.style.whiteSpace = expanded ? WhiteSpace.Normal : WhiteSpace.NoWrap;
			});
		}
	}
}
