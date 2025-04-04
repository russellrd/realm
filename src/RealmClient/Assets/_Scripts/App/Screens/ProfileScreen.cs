using UnityEngine.UIElements;

namespace Realm
{
    class ProfileScreen : VisualElement
    {
        private readonly Unity.AppUI.UI.Text userTypeLabel;
        private readonly Unity.AppUI.UI.Text userOrganizationLabel;

        public ProfileScreen()
        {
            AddToClassList("text-center");

            var heading = new Unity.AppUI.UI.Heading(DatabaseController.GetCurrentUserName());
            heading.AddToClassList("text-center");
            Add(heading);

            userTypeLabel = new Unity.AppUI.UI.Text("");
            userTypeLabel.style.fontSize = 14;
            userTypeLabel.AddToClassList("text-center");
            userTypeLabel.AddToClassList("spaced-below");
            Add(userTypeLabel);

            userOrganizationLabel = new Unity.AppUI.UI.Text("");
            userOrganizationLabel.AddToClassList("text-center");
            userOrganizationLabel.AddToClassList("spaced-below");
            Add(userOrganizationLabel);

            var settingsButton = new PressableRow("Settings");
            settingsButton.AddToClassList("spaced-below");
            Add(settingsButton);

            var logoutButton = new Unity.AppUI.UI.Button { title = "Logout" };
            logoutButton.clicked += () =>
            {
                DatabaseController.Logout();
                NavigationController.NavigateTo(NavigationController.Destinations.login);
            };
            settingsButton.AddToClassList("spaced-below");
            Add(logoutButton);

            Load();
        }

        protected async void Load()
        {
            if (DatabaseController.GetCurrentUserType() == "organization")
            {
                userTypeLabel.RemoveFromClassList("spaced-below");
                userTypeLabel.text = "Organization User";
                userOrganizationLabel.text = $"({await DatabaseController.GetCurrentUserOrganizationNameAsync()})";
            }
            else
                userTypeLabel.text = "General User";
        }
    }
}