using UnityEngine.UIElements;

namespace Realm
{
    class CreateTourScreen : VisualElement
    {
        private readonly Unity.AppUI.UI.TextField nameField;
        private readonly Unity.AppUI.UI.TextField descriptionField;
        private readonly Unity.AppUI.UI.Text errorText;
        private GPSCoordinate startCoordinate;

        public CreateTourScreen()
        {
            Add(new BackButton(NavigationController.Destinations.tours));

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
                // TODO: Hide UI
                NavigationDisplay.SwitchToRealm(NavigationController.Destinations.create_tour, true);
            };
            enterRealmButton.AddToClassList("large-spaced-below");
            Add(enterRealmButton);

            var createButton = new Unity.AppUI.UI.Button { title = "Create" };
            createButton.clicked += () => HandleCreate();
            createButton.AddToClassList("space-below");
            Add(createButton);

            errorText = new Unity.AppUI.UI.Text("");
            errorText.AddToClassList("error-text");
            Add(errorText);
        }

        private async void HandleCreate()
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
            var success = await DatabaseController.CreateTour(
                nameField.value,
                descriptionField.value,
                startCoordinate.Latitude,
                startCoordinate.Longitude
            );
            if (success)
                NavigationController.NavigateTo(NavigationController.Destinations.tours);
            else
                errorText.text = "Could not create tour";
        }
    }
}