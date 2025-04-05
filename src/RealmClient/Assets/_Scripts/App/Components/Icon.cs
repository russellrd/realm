using UnityEngine;
using UnityEngine.UIElements;

namespace Realm
{
    class Icon : VisualElement
    {
        public Icon(Sprite sprite, int size = 50)
        {
            style.backgroundImage = new StyleBackground(sprite);
            style.height = size;
            style.width = size;
        }
    }
}