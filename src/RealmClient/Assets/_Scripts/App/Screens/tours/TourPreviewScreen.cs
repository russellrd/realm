using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Realm
{
    class TourPreviewScreen : VisualElement
    {
        public TourPreviewScreen(Dictionary<string, string> data)
        {
            Add(new BackButton(NavigationController.Destinations.tours));

            Load(data);
        }

        public async void Load(Dictionary<string, string> data)
        {
            string tourId;
            if (data.TryGetValue("tourId", out tourId))
            {
                var tour = await DatabaseController.GetTourFromId(tourId);

                var text = new Unity.AppUI.UI.Text(tour.Name);
                text.style.fontSize = 20;
                text.AddToClassList("spaced-below");
                Add(text);

                var description = new Unity.AppUI.UI.Text(tour.Description);
                description.style.fontSize = 14;
                description.AddToClassList("spaced-below");
                Add(description);

                // TODO: Add Map Preivew

                var startButton = new Unity.AppUI.UI.Button { title = "Start" };
                startButton.clicked += () =>
                {
                    NavigationDisplay.SwitchToRealm(NavigationController.Destinations.tour_preview, false, data);
                };
                Add(startButton);
            }
        }
    }
}