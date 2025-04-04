using UnityEngine;
using UnityEngine.UIElements;

namespace Realm
{
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
}