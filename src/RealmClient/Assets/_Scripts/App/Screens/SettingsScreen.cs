using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Realm
{
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
}