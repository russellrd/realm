using UnityEngine;
using UnityEngine.UIElements;

namespace Realm
{
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
}