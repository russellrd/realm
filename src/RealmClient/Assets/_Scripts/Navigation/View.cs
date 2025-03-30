using Unity.AppUI.Navigation;
using Unity.AppUI.UI;
using UnityEngine.Scripting;
using UnityEngine.UIElements;
using Unity.AppUI.Navigation.Generated;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Unity.AppUI.Core;

namespace Realm.Navigation
{
    // [Preserve]
    // class SplashScreen : NavigationScreen
    // {
    //     public SplashScreen()
    //     {
    //         var content = new Preloader();
    //         content.StretchToParentSize();

    //         var theme = SettingsScreen.themeOptions[PlayerPrefs.GetInt("SETTING_THEME", 0)].ToLower();

    //         hierarchy.Add(content);

    //         schedule.Execute((timer) =>
    //         {
    //             this.GetContextProvider<ThemeContext>().ProvideContext(new ThemeContext(theme));
    //             if (DatabaseController.IsValidUser())
    //                 this.FindNavController().Navigate(Actions.splash_to_tours);
    //             else
    //                 this.FindNavController().Navigate(Actions.splash_to_auth);
    //         }).ExecuteLater(2000);
    //     }
    // }

    // class TourButton : VisualElement
    // {
    //     public TourButton(string tourName, string tourId)
    //     {
    //         AddToClassList("pressable-row");
    //         var tourLabel = new Unity.AppUI.UI.Text
    //         {
    //             text = tourName,
    //             size = TextSize.L
    //         };
    //         var tourPressable = new Pressable(() =>
    //         {
    //             this.FindNavController()
    //                 .Navigate(
    //                     Actions.tours_to_tour_preview,
    //                     new Argument("tourId", tourId)
    //                 );
    //         });
    //         tourLabel.AddToClassList("pressable-row-title");
    //         tourLabel.AddManipulator(tourPressable);
    //         Add(tourLabel);

    //         var editIcon = new Unity.AppUI.UI.Icon
    //         {
    //             iconName = "plumber",
    //             size = IconSize.M,
    //             pickingMode = PickingMode.Position
    //         };
    //         var editPressable = new Pressable(() =>
    //         {
    //             this.FindNavController()
    //                 .Navigate(
    //                     Actions.tours_to_edit_tour,
    //                     new Argument("tourId", tourId)
    //                 );
    //         });
    //         editIcon.AddToClassList("pressable-row-chevron");
    //         editIcon.AddManipulator(editPressable);
    //         Add(editIcon);
    //     }
    // }

    // [Preserve]
    // class ToursScreen : NavigationScreen
    // {
    //     public ToursScreen()
    //     {
    //         var newTourButton = new Unity.AppUI.UI.Button { title = "Create a New Tour" };
    //         newTourButton.AddToClassList("spaced-below");
    //         newTourButton.clicked += () => this.FindNavController().Navigate(Actions.tours_to_create_tour);
    //         Add(newTourButton);
    //     }

    //     protected override async void OnEnter(NavController controller, NavDestination destination, Argument[] args)
    //     {
    //         var tours = await DatabaseController.GetAllTours();
    //         foreach (TourDTO tour in tours)
    //         {
    //             var tourPressable = new TourButton(tour.Name, tour.Id);
    //             Add(tourPressable);
    //         }
    //     }
    // }

    // [Preserve]
    // class TourPreviewScreen : NavigationScreen
    // {
    //     public TourPreviewScreen() { }

    //     protected override async void OnEnter(NavController controller, NavDestination destination, Argument[] args)
    //     {
    //         var tourId = args.FirstOrDefault(arg => arg.name == "tourId");
    //         if (tourId != null)
    //         {
    //             var tour = await DatabaseController.GetTourFromId(tourId.value);
    //             var text = new Unity.AppUI.UI.Text(tour.Name)
    //             {
    //                 size = TextSize.XXL
    //             };
    //             Add(text);
    //         }
    //         // TODO: Add Map Preivew
    //         var startButton = new Unity.AppUI.UI.Button { title = "Start" };
    //         startButton.clicked += () =>
    //         {
    //             // TODO: Hide UI
    //             // TODO: Start tour in General User mode
    //         };
    //         Add(startButton);
    //     }
    // }

    // [Preserve]
    // class CreateTourScreen : NavigationScreen
    // {
    //     private readonly Unity.AppUI.UI.TextField nameField;
    //     private readonly Unity.AppUI.UI.TextField descriptionField;
    //     private readonly Unity.AppUI.UI.Text errorText;

    //     private GPSCoordinate startCoordinate;

    //     public CreateTourScreen()
    //     {
    //         Add(new Unity.AppUI.UI.Text { text = "Name", size = TextSize.L });

    //         nameField = new Unity.AppUI.UI.TextField
    //         {
    //             placeholder = "Name",
    //             maxLength = 20
    //         };
    //         nameField.AddToClassList("spaced-below");
    //         Add(nameField);

    //         Add(new Unity.AppUI.UI.Text { text = "Description", size = TextSize.L });

    //         descriptionField = new Unity.AppUI.UI.TextField
    //         {
    //             placeholder = "Description"
    //         };
    //         descriptionField.AddToClassList("spaced-below");
    //         Add(descriptionField);

    //         var setStartPointButton = new Unity.AppUI.UI.Button { title = "Press to Set Start Point" };
    //         setStartPointButton.clicked += () =>
    //         {
    //             startCoordinate = RealWorldController.GetCurrentGPSCoordinates();
    //             setStartPointButton.title = startCoordinate.ToString();
    //             setStartPointButton.AddToClassList("completed-button");
    //         };
    //         setStartPointButton.AddToClassList("large-spaced-below");
    //         Add(setStartPointButton);

    //         var enterRealmButton = new Unity.AppUI.UI.Button { title = "Enter the Realm" };
    //         enterRealmButton.clicked += () =>
    //         {
    //             // TODO: Hide UI
    //             // TODO: Create a tour in Organization User mode
    //         };
    //         enterRealmButton.AddToClassList("large-spaced-below");
    //         Add(enterRealmButton);

    //         var createButton = new Unity.AppUI.UI.Button { title = "Create" };
    //         createButton.clicked += () => HandleCreate();
    //         createButton.AddToClassList("space-below");
    //         Add(createButton);

    //         errorText = new Unity.AppUI.UI.Text("");
    //         errorText.AddToClassList("error-text");
    //         Add(errorText);
    //     }

    //     private async void HandleCreate()
    //     {
    //         if (nameField.value == null)
    //         {
    //             errorText.text = "Name field empty";
    //             return;
    //         }
    //         if (descriptionField.value == null)
    //         {
    //             errorText.text = "Description field empty";
    //             return;
    //         }
    //         if (startCoordinate == null)
    //         {
    //             errorText.text = "Start Point not set";
    //             return;
    //         }
    //         var success = await DatabaseController.CreateTour(
    //             nameField.value,
    //             descriptionField.value,
    //             startCoordinate.Latitude,
    //             startCoordinate.Longitude
    //         );
    //         if (success)
    //             this.FindNavController().Navigate(Actions.go_to_tours);
    //         else
    //             errorText.text = "Could not create tour";
    //     }
    // }

    // [Preserve]
    // class EditTourScreen : NavigationScreen
    // {
    //     private Unity.AppUI.UI.TextField nameField;
    //     private Unity.AppUI.UI.TextField descriptionField;
    //     private Unity.AppUI.UI.Text errorText;

    //     private GPSCoordinate startCoordinate;

    //     public EditTourScreen()
    //     {
    //         Add(new Unity.AppUI.UI.Text("Edit Tour"));
    //     }

    //     protected override async void OnEnter(NavController controller, NavDestination destination, Argument[] args)
    //     {
    //         Add(new Unity.AppUI.UI.Text { text = "Name", size = TextSize.L });

    //         nameField = new Unity.AppUI.UI.TextField
    //         {
    //             placeholder = "Name",
    //             maxLength = 20
    //         };
    //         nameField.AddToClassList("spaced-below");
    //         Add(nameField);

    //         Add(new Unity.AppUI.UI.Text { text = "Description", size = TextSize.L });

    //         descriptionField = new Unity.AppUI.UI.TextField
    //         {
    //             placeholder = "Description"
    //         };
    //         descriptionField.AddToClassList("spaced-below");
    //         Add(descriptionField);

    //         var setStartPointButton = new Unity.AppUI.UI.Button { title = "Press to Set Start Point" };
    //         setStartPointButton.clicked += () =>
    //         {
    //             startCoordinate = RealWorldController.GetCurrentGPSCoordinates();
    //             setStartPointButton.title = startCoordinate.ToString();
    //             setStartPointButton.AddToClassList("completed-button");
    //         };
    //         setStartPointButton.AddToClassList("large-spaced-below");
    //         Add(setStartPointButton);

    //         var enterRealmButton = new Unity.AppUI.UI.Button { title = "Enter the Realm" };
    //         enterRealmButton.clicked += () =>
    //         {
    //             // TODO: Hide UI
    //             // TODO: Edit existing tour in Organization User mode
    //         };
    //         enterRealmButton.AddToClassList("large-spaced-below");
    //         Add(enterRealmButton);

    //         var tourId = args.FirstOrDefault(arg => arg.name == "tourId");
    //         if (tourId != null)
    //         {
    //             var tour = await DatabaseController.GetTourFromId(tourId.value);
    //             nameField.value = tour.Name;
    //             descriptionField.value = tour.Description;
    //             startCoordinate = new GPSCoordinate(tour.StartLatitude, tour.StartLongitude);

    //             setStartPointButton.title = startCoordinate.ToString();
    //             setStartPointButton.AddToClassList("completed-button");

    //             var saveButton = new Unity.AppUI.UI.Button { title = "Save" };
    //             saveButton.clicked += () => HandleSave(tourId.value);
    //             saveButton.AddToClassList("space-below");
    //             Add(saveButton);
    //         }

    //         errorText = new Unity.AppUI.UI.Text("");
    //         errorText.AddToClassList("error-text");
    //         Add(errorText);
    //     }

    //     private async void HandleSave(string tourId)
    //     {
    //         if (nameField.value == null)
    //         {
    //             errorText.text = "Name field empty";
    //             return;
    //         }
    //         if (descriptionField.value == null)
    //         {
    //             errorText.text = "Description field empty";
    //             return;
    //         }
    //         if (startCoordinate == null)
    //         {
    //             errorText.text = "Start Point not set";
    //             return;
    //         }
    //         var success = await DatabaseController.UpdateTourFromId(
    //             tourId,
    //             nameField.value,
    //             descriptionField.value,
    //             startCoordinate.Latitude,
    //             startCoordinate.Longitude
    //         );
    //         if (success)
    //             this.FindNavController().Navigate(Actions.go_to_tours);
    //         else
    //             errorText.text = "Could not save tour";
    //     }
    // }

    // [Preserve]
    // class LoginScreen : NavigationScreen
    // {
    //     private readonly Unity.AppUI.UI.TextField emailField;
    //     private readonly Unity.AppUI.UI.TextField passwordField;
    //     private readonly Unity.AppUI.UI.Text errorText;

    //     public LoginScreen()
    //     {
    //         Add(new Unity.AppUI.UI.Heading("Login"));

    //         emailField = new Unity.AppUI.UI.TextField { placeholder = "Email" };
    //         Add(emailField);

    //         passwordField = new Unity.AppUI.UI.TextField
    //         {
    //             placeholder = "Password",
    //             isPassword = true,
    //             maxLength = 20
    //         };
    //         Add(passwordField);

    //         errorText = new Unity.AppUI.UI.Text("");
    //         errorText.AddToClassList("error-text");
    //         Add(errorText);

    //         var loginButton = new Unity.AppUI.UI.Button { title = "Login" };
    //         loginButton.clicked += () => HandleLogin();
    //         Add(loginButton);

    //         var registerButton = new Unity.AppUI.UI.Button { title = "Register" };
    //         registerButton.style.marginTop = 50;
    //         registerButton.clicked += () => this.FindNavController().Navigate(Actions.login_to_register);
    //         Add(registerButton);
    //     }

    //     private async void HandleLogin()
    //     {
    //         if (emailField.value == null)
    //         {
    //             errorText.text = "Email field empty";
    //             return;
    //         }
    //         if (passwordField.value == null)
    //         {
    //             errorText.text = "Password field empty";
    //             return;
    //         }
    //         await DatabaseController.AuthenticateUser(emailField.value, passwordField.value);
    //         if (DatabaseController.IsValidUser())
    //             this.FindNavController().Navigate(Actions.go_to_tours);
    //         else
    //             errorText.text = "Please contact the system administrator to verify as an Organization user";
    //     }
    // }

    // [Preserve]
    // class RegisterScreen : NavigationScreen
    // {
    //     private readonly Unity.AppUI.UI.TextField usernameField;
    //     private readonly Unity.AppUI.UI.TextField emailField;
    //     private readonly Unity.AppUI.UI.TextField passwordField;
    //     private readonly Unity.AppUI.UI.TextField passwordConfirmField;
    //     private readonly Unity.AppUI.UI.Dropdown userTypeDropdown;
    //     private readonly Unity.AppUI.UI.Dropdown organizationDropdown;
    //     private readonly Unity.AppUI.UI.Text errorText;

    //     private readonly List<string> userTypeOptions = new()
    //     {
    //         "General",
    //         "Organization"
    //     };

    //     private List<OrganizationDTO> organizations;

    //     public RegisterScreen()
    //     {
    //         Add(new Unity.AppUI.UI.Heading("Register"));

    //         usernameField = new Unity.AppUI.UI.TextField { placeholder = "Username" };
    //         Add(usernameField);

    //         emailField = new Unity.AppUI.UI.TextField { placeholder = "Email" };
    //         Add(emailField);

    //         passwordField = new Unity.AppUI.UI.TextField
    //         {
    //             placeholder = "Password",
    //             isPassword = true,
    //             maxLength = 20
    //         };
    //         Add(passwordField);

    //         passwordConfirmField = new Unity.AppUI.UI.TextField
    //         {
    //             placeholder = "Confirm Password",
    //             isPassword = true,
    //             maxLength = 20
    //         };
    //         Add(passwordConfirmField);

    //         userTypeDropdown = new Unity.AppUI.UI.Dropdown
    //         {
    //             bindItem = (item, i) => item.label = userTypeOptions[i],
    //             sourceItems = userTypeOptions
    //         };
    //         userTypeDropdown.SetValueWithoutNotify(new[] { 0 });
    //         userTypeDropdown.RegisterValueChangedCallback(evt =>
    //         {
    //             organizationDropdown.visible = userTypeOptions[evt.newValue.ToArray()[0]].ToLower() == "organization";
    //         });
    //         Add(userTypeDropdown);

    //         organizationDropdown = new Unity.AppUI.UI.Dropdown
    //         {
    //             visible = false
    //         };
    //         Add(organizationDropdown);

    //         errorText = new Unity.AppUI.UI.Text("");
    //         errorText.AddToClassList("error-text");
    //         Add(errorText);

    //         var registerButton = new Unity.AppUI.UI.Button { title = "Register" };
    //         registerButton.clicked += () => HandleRegister();
    //         Add(registerButton);
    //     }

    //     protected override async void OnEnter(NavController controller, NavDestination destination, Argument[] args)
    //     {
    //         organizations = await DatabaseController.GetAllOrganizations();
    //         foreach (OrganizationDTO organization in organizations)
    //         {
    //             organizationDropdown.bindItem = (item, i) => item.label = organizations[i].Name;
    //             organizationDropdown.sourceItems = organizations;
    //         }
    //     }

    //     private async void HandleRegister()
    //     {
    //         if (usernameField.value == null)
    //         {
    //             errorText.text = "Username field empty";
    //             return;
    //         }
    //         if (emailField.value == null)
    //         {
    //             errorText.text = "Email field empty";
    //             return;
    //         }
    //         if (passwordField.value == null)
    //         {
    //             errorText.text = "Password field empty";
    //             return;
    //         }
    //         if (passwordField.value.Length < 8)
    //         {
    //             errorText.text = "Password must be at least 8 characters";
    //         }
    //         if (passwordConfirmField.value == null)
    //         {
    //             errorText.text = "Confirm Password field empty";
    //             return;
    //         }
    //         if (passwordField.value != passwordConfirmField.value)
    //         {
    //             errorText.text = "Passwords do not match";
    //             return;
    //         }
    //         var userType = userTypeOptions[userTypeDropdown.value.ToArray()[0]].ToLower();
    //         if (userType == "organization" && organizationDropdown.value.ToArray().Length == 0)
    //         {
    //             errorText.text = "Select your organization";
    //             return;
    //         }
    //         var success = await DatabaseController.CreateNewUser(
    //             usernameField.value,
    //             emailField.value,
    //             passwordField.value,
    //             userType,
    //             userType == "organization" ? organizations[organizationDropdown.value.ToArray()[0]].Id : null
    //         );
    //         if (success)
    //             this.FindNavController().Navigate(Actions.go_to_tours);
    //         else
    //             errorText.text = "Could not register";
    //     }
    // }

    // class PressableRow : VisualElement
    // {
    //     public Pressable clickable { get; }

    //     public PressableRow(string title)
    //     {
    //         clickable = new Pressable();
    //         this.AddManipulator(clickable);

    //         AddToClassList("pressable-row");
    //         var titleLabel = new Unity.AppUI.UI.Text(title) { size = TextSize.L, pickingMode = PickingMode.Ignore };
    //         titleLabel.AddToClassList("pressable-row-title");
    //         Add(titleLabel);

    //         var chevronIcon = new Icon { iconName = "caret-right", size = IconSize.M, pickingMode = PickingMode.Ignore };
    //         chevronIcon.AddToClassList("pressable-row-chevron");
    //         Add(chevronIcon);
    //     }
    // }

    // [Preserve]
    // class ProfileScreen : NavigationScreen
    // {
    //     public ProfileScreen()
    //     {
    //         var settingsButton = new PressableRow("Settings");
    //         settingsButton.clickable.clicked += () =>
    //         {
    //             this.FindNavController().Navigate(Actions.profile_to_settings);
    //         };
    //         settingsButton.AddToClassList("spaced-below");
    //         Add(settingsButton);

    //         var logoutButton = new Unity.AppUI.UI.Button { title = "Logout" };
    //         logoutButton.clicked += () =>
    //         {
    //             DatabaseController.Logout();
    //             this.FindNavController().Navigate(Actions.go_to_login);
    //         };
    //         settingsButton.AddToClassList("spaced-below");
    //         Add(logoutButton);
    //     }
    // }

    // [Preserve]
    // class SettingsScreen : NavigationScreen
    // {
    //     private readonly Unity.AppUI.UI.Dropdown themeDropdown;
    //     private readonly Unity.AppUI.UI.Dropdown localizationDropdown;
    //     private readonly Unity.AppUI.UI.TextField textSizeField;

    //     public static readonly List<string> themeOptions = new()
    //     {
    //         "Dark",
    //         "Light"
    //     };

    //     private readonly List<string> localizationOptions = new()
    //     {
    //         "English",
    //         "French",
    //         "Hindi",
    //         "Mandarin",
    //         "Spanish"
    //     };

    //     public SettingsScreen()
    //     {
    //         themeDropdown = new Unity.AppUI.UI.Dropdown
    //         {
    //             bindItem = (item, i) => item.label = themeOptions[i],
    //             sourceItems = themeOptions
    //         };
    //         themeDropdown.SetValueWithoutNotify(new[] { PlayerPrefs.GetInt("SETTING_THEME", 0) });
    //         themeDropdown.RegisterValueChangedCallback(OnThemeChanged);
    //         Add(themeDropdown);

    //         localizationDropdown = new Unity.AppUI.UI.Dropdown
    //         {
    //             bindItem = (item, i) => item.label = localizationOptions[i],
    //             sourceItems = localizationOptions
    //         };
    //         localizationDropdown.SetValueWithoutNotify(new[] { 0 });
    //         localizationDropdown.RegisterValueChangedCallback(OnLocalizationChanged);
    //         Add(localizationDropdown);
    //     }

    //     void OnThemeChanged(ChangeEvent<IEnumerable<int>> evt)
    //     {
    //         var theme = themeOptions[evt.newValue.ToArray()[0]].ToLower();
    //         this.GetContextProvider<ThemeContext>().ProvideContext(new ThemeContext(theme));
    //         PlayerPrefs.SetInt("SETTING_THEME", evt.newValue.ToArray()[0]);
    //     }

    //     void OnLocalizationChanged(ChangeEvent<IEnumerable<int>> evt)
    //     {
    //         // TODO: Change localization
    //         // var language = localizationOptions[evt.newValue.ToArray()[0]].ToLower();
    //         // this.GetContextProvider<LangContext>().ProvideContext(new LangContext("en-CA"));
    //     }
    // }

    [Preserve]
    class ChangePasswordScreen : NavigationScreen
    {

    }

    class VisualController : INavVisualController
    {
        public void SetupAppBar(AppBar appBar, NavDestination destination, NavController navController)
        {
            if (!destination.showAppBar)
                return;

            appBar.title = destination.label;
            appBar.stretch = true;
            appBar.expandedHeight = 92;
        }

        public void SetupBottomNavBar(BottomNavBar bottomNavBar, NavDestination destination, NavController navController)
        {
            if (!destination.showBottomNavBar)
                return;

            var toursButton = new BottomNavBarItem("walk-mode", "Tours", () => navController.Navigate(Actions.go_to_tours))
            {
                isSelected = destination.name == Destinations.tours
            };
            bottomNavBar.Add(toursButton);

            var profileButton = new BottomNavBarItem("profile", "Profile", () => navController.Navigate(Actions.go_to_profile))
            {
                isSelected = destination.name == Destinations.profile
            };
            bottomNavBar.Add(profileButton);
        }

        public void SetupDrawer(Drawer drawer, NavDestination destination, NavController navController)
        {
            return;
        }
    }
}
