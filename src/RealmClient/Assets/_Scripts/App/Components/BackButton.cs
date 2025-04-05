using UnityEngine;
using UnityEngine.UIElements;

namespace Realm
{
    class BackButton : VisualElement
    {
        public BackButton(NavigationController.Destinations destinations)
        {
            AddToClassList("pressable-row");
            AddToClassList("back-button");

            var chevronIcon = new Icon(Resources.Load<Sprite>("Sprites/Backward"), 30) { pickingMode = PickingMode.Ignore };
            chevronIcon.AddToClassList("pressable-row-chevron");
            Add(chevronIcon);

            var backPressable = new Clickable(() =>
            {
                NavigationController.NavigateTo(destinations);
            });
            this.AddManipulator(backPressable);
        }
    }
}