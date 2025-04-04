using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Realm
{
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
                    NavigationDisplay.panel.Add(splashScreen);
                    lastScreen = splashScreen;
                    break;
                case Destinations.login:
                    var loginScreen = new LoginScreen();
                    loginScreen.StretchToParentSize();
                    NavigationDisplay.panel.Add(loginScreen);
                    lastScreen = loginScreen;
                    break;
                case Destinations.register:
                    var registerScreen = new RegisterScreen();
                    registerScreen.StretchToParentSize();
                    NavigationDisplay.panel.Add(registerScreen);
                    lastScreen = registerScreen;
                    break;
                case Destinations.tour_preview:
                    var tourPreviewScreen = new TourPreviewScreen(data);
                    tourPreviewScreen.StretchToParentSize();
                    NavigationDisplay.panel.Add(tourPreviewScreen);
                    lastScreen = tourPreviewScreen;
                    break;
                case Destinations.create_tour:
                    var createTourScreen = new CreateTourScreen();
                    createTourScreen.StretchToParentSize();
                    NavigationDisplay.panel.Add(createTourScreen);
                    lastScreen = createTourScreen;
                    break;
                case Destinations.edit_tour:
                    var editTourScreen = new EditTourScreen(data);
                    editTourScreen.StretchToParentSize();
                    NavigationDisplay.panel.Add(editTourScreen);
                    lastScreen = editTourScreen;
                    break;
                case Destinations.profile:
                    var profileScreen = new ProfileScreen();
                    profileScreen.StretchToParentSize();
                    NavigationDisplay.panel.Add(profileScreen);
                    lastScreen = profileScreen;
                    hasNavBar = true;
                    break;
                case Destinations.settings:
                    var settingsScreen = new SettingsScreen();
                    settingsScreen.StretchToParentSize();
                    NavigationDisplay.panel.Add(settingsScreen);
                    lastScreen = settingsScreen;
                    break;
                case Destinations.tours:
                default:
                    var toursScreen = new ToursScreen();
                    toursScreen.StretchToParentSize();
                    NavigationDisplay.panel.Add(toursScreen);
                    lastScreen = toursScreen;
                    hasNavBar = true;
                    break;
            }
            currentRoute = destination;
            if (hasNavBar)
            {
                bottomNav = new BottomNav();
                NavigationDisplay.panel.Add(bottomNav);
            }
        }

        static public void ClearScreen()
        {
            if (lastScreen != null)
            {
                NavigationDisplay.panel.Remove(lastScreen);
                lastScreen = null;
            }
            if (bottomNav != null)
            {
                NavigationDisplay.panel.Remove(bottomNav);
                bottomNav = null;
            }
        }
    }
}