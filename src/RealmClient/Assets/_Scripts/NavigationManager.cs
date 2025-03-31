// using Unity.AppUI.UI;
using UnityEngine.Scripting;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Realm.Controller;
using UnityEngine.SceneManagement;

namespace Realm
{
    public class NavigationManager : MonoBehaviour
    {
        [SerializeField]
        private UIDocument uiDocument;

        [SerializeField]
        private SwitchController switchController;

        public static VisualElement main;

        public static VisualElement container;

        public static VisualElement panel;

        void Start()
        {
            main = uiDocument.rootVisualElement;

            Show(NavigationController.Destinations.splash, null);
        }

        public void Show(NavigationController.Destinations dest, Dictionary<string, string> data)
        {
            Debug.Log("Show" + dest.ToString());
            container = new VisualElement();
            container.AddToClassList("container");
            container.StretchToParentSize();
            main.Add(container);

            panel = new VisualElement();
            panel.AddToClassList("sub-container");
            panel.StretchToParentSize();
            container.Add(panel);

            NavigationController.NavigateTo(dest, data);
        }

        public static void SwitchToRealm(NavigationController.Destinations uiScreen, bool editMode, Dictionary<string, string> data = null)
        {
            Debug.Log("SwitchToRealm");
            SwitchController.uiScreen = uiScreen;
            SwitchController.data = data;
            SwitchController.editMode = editMode;
            NavigationController.ClearScreen();
            main.Remove(container);
            main.Clear();
        }
    }

    class NewBottomNavBarItem : Unity.AppUI.UI.BottomNavBarItem
    {
        public NewBottomNavBarItem(Sprite icon, string label, System.Action clickHandler)
        {

            AddToClassList("appui-bottom-navbar-item");
            base.pickingMode = PickingMode.Position;
            focusable = true;
            base.tabIndex = 0;
            clickable = new Unity.AppUI.UI.Pressable(clickHandler);
            var m_Icon = new Icon(icon, 25)
            {
                pickingMode = PickingMode.Ignore
            };
            m_Icon.AddToClassList("appui-bottom-navbar-item__icon");
            base.hierarchy.Add(m_Icon);
            var m_Label = new Unity.AppUI.UI.LocalizedTextElement(label)
            {
                pickingMode = PickingMode.Ignore
            };
            m_Label.AddToClassList("appui-bottom-navbar-item__label");
            base.hierarchy.Add(m_Label);
            this.icon = "icon";
        }
    }

    class BottomNav : VisualElement
    {
        private Unity.AppUI.UI.BottomNavBar bottomNavBar;

        public BottomNav()
        {
            bottomNavBar = new();
            var toursButton = new NewBottomNavBarItem(Resources.LoadAll<Sprite>("Sprites/WalkMode")[0], "Tours", () => NavigationController.NavigateTo(NavigationController.Destinations.tours))
            {
                isSelected = NavigationController.currentRoute == NavigationController.Destinations.tours
            };
            bottomNavBar.Add(toursButton);

            var mapsButton = new NewBottomNavBarItem(Resources.LoadAll<Sprite>("Sprites/MapToolIcon")[0], "Map", () => NavigationController.NavigateTo(NavigationController.Destinations.map))
            {
                isSelected = NavigationController.currentRoute == NavigationController.Destinations.map
            };
            bottomNavBar.Add(mapsButton);

            var profileButton = new NewBottomNavBarItem(Resources.LoadAll<Sprite>("Sprites/Profile")[0], "Profile", () => NavigationController.NavigateTo(NavigationController.Destinations.profile))
            {
                isSelected = NavigationController.currentRoute == NavigationController.Destinations.profile
            };
            bottomNavBar.Add(profileButton);

            AddToClassList("bottom-nav");

            Add(bottomNavBar);
        }
    }

    class Icon : VisualElement
    {
        public Icon(Sprite sprite, int size = 50)
        {
            style.backgroundImage = new StyleBackground(sprite);
            style.height = size;
            style.width = size;
            // AddToClassList("icon");
        }
    }

    class BackButton : VisualElement
    {
        public BackButton(NavigationController.Destinations destinations)
        {
            AddToClassList("pressable-row");
            AddToClassList("back-button");

            var chevronIcon = new Icon(Resources.LoadAll<Sprite>("Sprites/Backward")[0], 30) { pickingMode = PickingMode.Ignore };
            chevronIcon.AddToClassList("pressable-row-chevron");
            Add(chevronIcon);

            var backPressable = new Clickable(() =>
            {
                NavigationController.NavigateTo(destinations);
            });
            this.AddManipulator(backPressable);
        }
    }

    class SplashScreen : VisualElement
    {
        public SplashScreen()
        {
            schedule.Execute((timer) =>
            {
                // this.GetContextProvider<ThemeContext>().ProvideContext(new ThemeContext(theme));
                if (DatabaseController.IsValidUser())
                    NavigationController.NavigateTo(NavigationController.Destinations.tours);
                else
                    NavigationController.NavigateTo(NavigationController.Destinations.login);
            }).ExecuteLater(3000);
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

    [Preserve]
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
                    NavigationManager.SwitchToRealm(NavigationController.Destinations.tour_preview, false, data);
                };
                Add(startButton);
            }
        }
    }

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
                NavigationManager.SwitchToRealm(NavigationController.Destinations.create_tour, true);
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

    [Preserve]
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
                NavigationManager.SwitchToRealm(NavigationController.Destinations.edit_tour, true, data);
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

    [Preserve]
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

    [Preserve]
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

    class PressableRow : VisualElement
    {
        public Clickable clickable { get; }

        public PressableRow(string title)
        {
            clickable = new Clickable(() =>
            {
                NavigationController.NavigateTo(NavigationController.Destinations.settings);
            });
            this.AddManipulator(clickable);

            AddToClassList("pressable-row");
            var titleLabel = new Unity.AppUI.UI.Text(title) { pickingMode = PickingMode.Ignore };
            titleLabel.AddToClassList("pressable-row-title");
            Add(titleLabel);

            var chevronIcon = new Icon(Resources.LoadAll<Sprite>("Sprites/CaretRight")[0], 30) { pickingMode = PickingMode.Ignore };
            chevronIcon.AddToClassList("pressable-row-chevron");
            chevronIcon.AddToClassList("pressable-row-chevron-end");
            chevronIcon.style.paddingRight = 5;
            Add(chevronIcon);
        }
    }

    [Preserve]
    class ProfileScreen : VisualElement
    {
        public ProfileScreen()
        {
            var heading = new Unity.AppUI.UI.Heading(DatabaseController.GetCurrentUserName());
            heading.AddToClassList("spaced-below");
            Add(heading);

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
        }
    }

    [Preserve]
    class SettingsScreen : VisualElement
    {
        private readonly DropdownField themeDropdown;
        private readonly DropdownField localizationDropdown;
        private readonly TextField textSizeField;

        public static readonly List<string> themeOptions = new()
        {
            "Dark",
            "Light"
        };

        private readonly List<string> localizationOptions = new()
        {
            "English",
            "French",
            "Hindi",
            "Mandarin",
            "Spanish"
        };

        public SettingsScreen()
        {
            Add(new BackButton(NavigationController.Destinations.profile));

            Add(new Unity.AppUI.UI.Text { text = "Theme" });

            themeDropdown = new DropdownField(themeOptions, 0);
            themeDropdown.SetValueWithoutNotify(PlayerPrefs.GetString("SETTING_THEME", "Dark"));
            themeDropdown.RegisterValueChangedCallback(OnThemeChanged);
            Add(themeDropdown);

            Add(new Unity.AppUI.UI.Text { text = "Language" });

            localizationDropdown = new DropdownField(localizationOptions, 0);
            localizationDropdown.SetValueWithoutNotify(PlayerPrefs.GetString("SETTING_LANG", "English"));
            localizationDropdown.RegisterValueChangedCallback(OnLocalizationChanged);
            Add(localizationDropdown);
        }

        void OnThemeChanged(ChangeEvent<string> evt)
        {
            //     var theme = themeOptions[evt.newValue.ToArray()[0]].ToLower();
            // this.GetContextProvider<ThemeContext>().ProvideContext(new ThemeContext(theme));
            PlayerPrefs.SetString("SETTING_THEME", evt.newValue);
        }

        void OnLocalizationChanged(ChangeEvent<string> evt)
        {
            // TODO: Change localization
            // var language = localizationOptions[evt.newValue.ToArray()[0]].ToLower();
            // this.GetContextProvider<LangContext>().ProvideContext(new LangContext("en-CA"));
        }
    }

    public class NavigationController
    {
        private static VisualElement lastScreen;
        private static BottomNav bottomNav;
        public static Destinations currentRoute;

        public enum Destinations
        {
            none,
            splash,
            login,
            tours,
            profile,
            tour_preview,
            settings,
            create_tour,
            register,
            edit_tour,
            map
        }

        public static void NavigateTo(Destinations destination, Dictionary<string, string> data = null)
        {
            ClearScreen();

            bool hasNavBar = false;

            switch (destination)
            {
                case Destinations.map:
                    SceneManager.LoadScene("Map");
                    break;
                case Destinations.splash:
                    var splashScreen = new SplashScreen();
                    splashScreen.StretchToParentSize();
                    NavigationManager.panel.Add(splashScreen);
                    lastScreen = splashScreen;
                    break;
                case Destinations.login:
                    var loginScreen = new LoginScreen();
                    loginScreen.StretchToParentSize();
                    NavigationManager.panel.Add(loginScreen);
                    lastScreen = loginScreen;
                    break;
                case Destinations.register:
                    var registerScreen = new RegisterScreen();
                    registerScreen.StretchToParentSize();
                    NavigationManager.panel.Add(registerScreen);
                    lastScreen = registerScreen;
                    break;
                case Destinations.tour_preview:
                    var tourPreviewScreen = new TourPreviewScreen(data);
                    tourPreviewScreen.StretchToParentSize();
                    NavigationManager.panel.Add(tourPreviewScreen);
                    lastScreen = tourPreviewScreen;
                    break;
                case Destinations.create_tour:
                    var createTourScreen = new CreateTourScreen();
                    createTourScreen.StretchToParentSize();
                    NavigationManager.panel.Add(createTourScreen);
                    lastScreen = createTourScreen;
                    break;
                case Destinations.edit_tour:
                    var editTourScreen = new EditTourScreen(data);
                    editTourScreen.StretchToParentSize();
                    NavigationManager.panel.Add(editTourScreen);
                    lastScreen = editTourScreen;
                    break;
                case Destinations.profile:
                    var profileScreen = new ProfileScreen();
                    profileScreen.StretchToParentSize();
                    NavigationManager.panel.Add(profileScreen);
                    lastScreen = profileScreen;
                    hasNavBar = true;
                    break;
                case Destinations.settings:
                    var settingsScreen = new SettingsScreen();
                    settingsScreen.StretchToParentSize();
                    NavigationManager.panel.Add(settingsScreen);
                    lastScreen = settingsScreen;
                    break;
                case Destinations.tours:
                default:
                    var toursScreen = new ToursScreen();
                    toursScreen.StretchToParentSize();
                    NavigationManager.panel.Add(toursScreen);
                    lastScreen = toursScreen;
                    hasNavBar = true;
                    break;
            }
            currentRoute = destination;
            if (hasNavBar)
            {
                bottomNav = new BottomNav();
                NavigationManager.panel.Add(bottomNav);
            }
        }

        static public void ClearScreen()
        {
            if (lastScreen != null)
            {
                NavigationManager.panel.Remove(lastScreen);
                lastScreen = null;
            }
            if (bottomNav != null)
            {
                NavigationManager.panel.Remove(bottomNav);
                bottomNav = null;
            }
        }
    }
}
