using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Realm
{
    class ToursScreen : VisualElement
    {
        public ToursScreen()
        {
            var newTourButton = new Unity.AppUI.UI.Button { title = "Create a New Tour" };
            newTourButton.AddToClassList("button");
            newTourButton.AddToClassList("spaced-below");
            newTourButton.clicked += () => NavigationController.NavigateTo(NavigationController.Destinations.create_tour);
            Add(newTourButton);

            Load();
        }

        protected async void Load()
        {
            var tours = await DatabaseController.GetAllTours();
            foreach (TourDTO tour in tours)
            {
                var tourPressable = new TourButton(tour.Name, tour.Id);
                Add(tourPressable);
            }
        }
    }

    class TourButton : VisualElement
    {
        public TourButton(string tourName, string tourId)
        {
            AddToClassList("pressable-row");
            var tourLabel = new Unity.AppUI.UI.Text
            {
                text = tourName
            };
            var tourPressable = new Clickable(() =>
            {
                NavigationController.NavigateTo(NavigationController.Destinations.tour_preview, new Dictionary<string, string> { { "tourId", tourId } });
            });
            tourLabel.AddToClassList("pressable-row-title");
            tourLabel.AddManipulator(tourPressable);
            Add(tourLabel);

            var editIcon = new Icon(Resources.LoadAll<Sprite>("Sprites/Plumber")[0], 25)
            {
                pickingMode = PickingMode.Position
            };
            var editPressable = new Clickable(() =>
            {
                NavigationController.NavigateTo(NavigationController.Destinations.edit_tour, new Dictionary<string, string> { { "tourId", tourId } });
            });
            editIcon.AddToClassList("pressable-row-chevron");
            editIcon.AddToClassList("pressable-row-chevron-end");
            editIcon.AddManipulator(editPressable);
            Add(editIcon);
        }
    }
}