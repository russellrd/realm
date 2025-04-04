using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Realm
{
    class EditTourScreen : VisualElement
    {
        private Unity.AppUI.UI.TextField nameField;
        private Unity.AppUI.UI.TextField descriptionField;
        private Unity.AppUI.UI.Text errorText;

        private GPSCoordinate startCoordinate;

        public EditTourScreen(Dictionary<string, string> data)
        {
            Add(new BackButton(NavigationController.Destinations.tours));

            Load(data);
        }

        public async void Load(Dictionary<string, string> data)
        {
            Add(new Unity.AppUI.UI.Heading("Edit Tour"));

            Add(new Unity.AppUI.UI.Text { text = "Name" });

            nameField = new Unity.AppUI.UI.TextField
            {
                placeholder = "Name",
                maxLength = 20
            };
            nameField.AddToClassList("spaced-below");
            Add(nameField);

            Add(new Unity.AppUI.UI.Text { text = "Description" });

            descriptionField = new Unity.AppUI.UI.TextField
            {
                placeholder = "Description"
            };
            descriptionField.AddToClassList("spaced-below");
            Add(descriptionField);

            var setStartPointButton = new Unity.AppUI.UI.Button { title = "Press to Set Start Point" };
            setStartPointButton.clicked += () =>
            {
                startCoordinate = RealWorldController.GetCurrentGPSCoordinates();
                setStartPointButton.title = startCoordinate.ToString();
                setStartPointButton.AddToClassList("completed-button");
            };
            setStartPointButton.AddToClassList("large-spaced-below");
            Add(setStartPointButton);

            var enterRealmButton = new Unity.AppUI.UI.Button { title = "Enter the Realm" };
            enterRealmButton.clicked += () =>
            {
                NavigationDisplay.SwitchToRealm(NavigationController.Destinations.edit_tour, true, data);
            };
            enterRealmButton.AddToClassList("large-spaced-below");
            Add(enterRealmButton);

            string tourId;
            if (data.TryGetValue("tourId", out tourId))
            {
                var tour = await DatabaseController.GetTourFromId(tourId);
                nameField.value = tour.Name;
                descriptionField.value = tour.Description;
                startCoordinate = new GPSCoordinate(tour.StartLatitude, tour.StartLongitude);

                setStartPointButton.title = startCoordinate.ToString();
                setStartPointButton.AddToClassList("completed-button");

                var saveButton = new Unity.AppUI.UI.Button { title = "Save" };
                saveButton.clicked += () => HandleSave(tourId);
                saveButton.AddToClassList("space-below");
                Add(saveButton);
            }

            errorText = new Unity.AppUI.UI.Text("");
            errorText.AddToClassList("error-text");
            Add(errorText);
        }

        private async void HandleSave(string tourId)
        {
            if (nameField.value == null)
            {
                errorText.text = "Name field empty";
                return;
            }
            if (descriptionField.value == null)
            {
                errorText.text = "Description field empty";
                return;
            }
            if (startCoordinate == null)
            {
                errorText.text = "Start Point not set";
                return;
            }
            var success = await DatabaseController.UpdateTourFromId(
                tourId,
                nameField.value,
                descriptionField.value,
                startCoordinate.Latitude,
                startCoordinate.Longitude
            );
            if (success)
                NavigationController.NavigateTo(NavigationController.Destinations.tours);
            else
                errorText.text = "Could not save tour";
        }
    }
}