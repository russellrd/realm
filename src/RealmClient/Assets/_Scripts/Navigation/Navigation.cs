using UnityEngine;
using UnityEngine.UIElements;
using Unity.AppUI.Navigation;

namespace Realm.Navigation
{
    public class Navigation : MonoBehaviour
    {
        [SerializeField]
        private UIDocument uiDocument;

        [SerializeField]
        private NavGraphViewAsset graphViewAsset;

        void Start()
        {
            var navHost = new NavHost();
            navHost.navController.SetGraph(graphViewAsset);
            navHost.visualController = new VisualController();

            var panel = new Unity.AppUI.UI.Panel
            {
                scale = "large"
            };
            uiDocument.rootVisualElement.Add(panel);
            panel.StretchToParentSize();

            panel.Add(navHost);
            navHost.StretchToParentSize();
        }
    }
}
