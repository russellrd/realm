using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Realm
{
    class RegisterScreen : VisualElement
    {
        private readonly Unity.AppUI.UI.TextField usernameField;
        private readonly Unity.AppUI.UI.TextField emailField;
        private readonly Unity.AppUI.UI.TextField passwordField;
        private readonly Unity.AppUI.UI.TextField passwordConfirmField;
        private readonly DropdownField userTypeDropdown;
        private readonly DropdownField organizationDropdown;
        private readonly Unity.AppUI.UI.Text errorText;

        private readonly List<string> userTypeOptions = new()
        {
            "General",
            "Organization"
        };

        private List<OrganizationDTO> organizations;

        public RegisterScreen()
        {
            Add(new BackButton(NavigationController.Destinations.login));

            Add(new Unity.AppUI.UI.Heading("Register"));

            usernameField = new Unity.AppUI.UI.TextField { placeholder = "Username" };
            Add(usernameField);

            emailField = new Unity.AppUI.UI.TextField { placeholder = "Email" };
            Add(emailField);

            passwordField = new Unity.AppUI.UI.TextField
            {
                placeholder = "Password",
                isPassword = true,
                maxLength = 20
            };
            Add(passwordField);

            passwordConfirmField = new Unity.AppUI.UI.TextField
            {
                placeholder = "Confirm Password",
                isPassword = true,
                maxLength = 20
            };
            Add(passwordConfirmField);

            Add(new Unity.AppUI.UI.Text { text = "Language" });

            userTypeDropdown = new DropdownField(userTypeOptions, 0);
            userTypeDropdown.RegisterValueChangedCallback(evt =>
            {
                organizationDropdown.visible = evt.newValue.ToLower() == "organization";
                organizationDropdown.choices = organizations.Select(o => o.Name).ToList();
            });
            Add(userTypeDropdown);

            organizationDropdown = new DropdownField()
            {
                visible = false
            };
            Add(organizationDropdown);

            errorText = new Unity.AppUI.UI.Text("");
            errorText.AddToClassList("error-text");
            Add(errorText);

            var registerButton = new Unity.AppUI.UI.Button { title = "Register" };
            registerButton.clicked += () => HandleRegister();
            Add(registerButton);

            Load();
        }

        protected async void Load()
        {
            organizations = await DatabaseController.GetAllOrganizations();
            organizationDropdown.choices = organizations.Select(o => o.Name).ToList();
        }

        private async void HandleRegister()
        {
            if (usernameField.value == null)
            {
                errorText.text = "Username field empty";
                return;
            }
            if (emailField.value == null)
            {
                errorText.text = "Email field empty";
                return;
            }
            if (passwordField.value == null)
            {
                errorText.text = "Password field empty";
                return;
            }
            if (passwordField.value.Length < 8)
            {
                errorText.text = "Password must be at least 8 characters";
            }
            if (passwordConfirmField.value == null)
            {
                errorText.text = "Confirm Password field empty";
                return;
            }
            if (passwordField.value != passwordConfirmField.value)
            {
                errorText.text = "Passwords do not match";
                return;
            }
            var userType = userTypeDropdown.value.ToLower();
            if (userType == "organization" && organizationDropdown.value.ToArray().Length == 0)
            {
                errorText.text = "Select your organization";
                return;
            }
            var success = await DatabaseController.CreateNewUser(
                usernameField.value,
                emailField.value,
                passwordField.value,
                userType,
                userType == "organization" ? organizations[organizationDropdown.index].Id : null
            );
            if (success)
                NavigationController.NavigateTo(NavigationController.Destinations.tours);
            else
                errorText.text = "Could not register";
        }
    }
}