using UnityEngine.UIElements;

namespace Realm
{
    class LoginScreen : VisualElement
    {
        private readonly Unity.AppUI.UI.TextField emailField;
        private readonly Unity.AppUI.UI.TextField passwordField;
        private readonly Unity.AppUI.UI.Text errorText;

        public LoginScreen()
        {
            Add(new Unity.AppUI.UI.Heading("Login"));

            emailField = new Unity.AppUI.UI.TextField { placeholder = "Email" };
            emailField.AddToClassList("spaced-below");
            Add(emailField);

            passwordField = new Unity.AppUI.UI.TextField
            {
                placeholder = "Password",
                isPassword = true,
                maxLength = 20
            };
            passwordField.AddToClassList("spaced-below");
            Add(passwordField);

            errorText = new Unity.AppUI.UI.Text("");
            errorText.AddToClassList("error-text");
            Add(errorText);

            var loginButton = new Unity.AppUI.UI.Button { title = "Login" };
            loginButton.AddToClassList("spaced-below");
            loginButton.clicked += () => HandleLogin();
            Add(loginButton);

            var registerButton = new Unity.AppUI.UI.Button { title = "Register" };
            registerButton.style.marginTop = 50;
            registerButton.clicked += () => NavigationController.NavigateTo(NavigationController.Destinations.register);
            Add(registerButton);
        }

        private async void HandleLogin()
        {
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
            await DatabaseController.AuthenticateUser(emailField.value, passwordField.value);
            if (DatabaseController.IsValidUser())
                NavigationController.NavigateTo(NavigationController.Destinations.tours);
            else
                errorText.text = "Please contact the system administrator to verify as an Organization user";
        }
    }
}