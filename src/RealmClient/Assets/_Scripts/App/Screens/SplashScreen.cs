using UnityEngine.UIElements;

namespace Realm
{
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
}