// Copyright 2024 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// ArcGISMapsSDK

// @@Start(namespaces)
using UnityEngine;
using UnityEngine.UI;

using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils;
using Esri.GameEngine.MapView;
// @@End(namespaces)

public class DisplayDrawStatusCreator : MonoBehaviour
{
	private Image progressIndicator;

	private ArcGISMapComponent mapComponent;

	private const float rotationSpeed = 300f;

	private void OnEnable()
	{
		progressIndicator = GameObject.Find("ProgressIndicator").GetComponent<Image>();
		mapComponent = gameObject.GetComponentInParent<ArcGISMapComponent>();

		if (!mapComponent || !mapComponent.View || !progressIndicator)
		{
			return;
		}

		// Set an initial value for the progress indicator.
		progressIndicator.enabled = mapComponent.View.DrawStatus == ArcGISDrawStatus.InProgress;

		// Enable the progress indicator according to the view's draw status.
		mapComponent.View.DrawStatusChanged += (ArcGISDrawStatus status) =>
		{
			ArcGISMainThreadScheduler.Instance().Schedule(() =>
			{
				progressIndicator.enabled = status != ArcGISDrawStatus.Completed;
			});
		};
	}

	private void OnDisable()
	{
		if (!mapComponent || !mapComponent.View)
		{
			return;
		}

		mapComponent.View.DrawStatusChanged -= null;
	}

	private void Update()
	{
		if (!progressIndicator)
		{
			return;
		}

		// Rotate the progress indicator counterclockwise.
		var angles = progressIndicator.rectTransform.eulerAngles;
		angles.z += rotationSpeed * Time.deltaTime;
		progressIndicator.rectTransform.eulerAngles = angles;
	}
}
