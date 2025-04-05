using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class Settings
{
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

    [UnitySetUp]
    public IEnumerable Setup()
    {
        LogAssert.ignoreFailingMessages = true;
        SceneManager.LoadScene("Assets/Scenes/RealmInterface.unity", LoadSceneMode.Single);
        yield return null;
        GameObject.Find("ARCore Extensions").SetActive(false);
    }

    // Has valid settings
    [UnityTest]
    public IEnumerator Test_SM1_1()
    {
        yield return null;
        Assert.IsTrue(
            PlayerPrefs.HasKey("SETTING_THEME"),
            "Settings - Theme not being initialized"
        );
        Assert.IsTrue(
            PlayerPrefs.HasKey("SETTING_LANGUAGE"),
            "Settings - Language not being initialized"
        );
    }

    // Has invalid settings
    [UnityTest]
    public IEnumerator Test_SM1_2()
    {
        yield return null;
        Assert.IsFalse(
            PlayerPrefs.HasKey("SETTING_PORTALWAVE"),
            "Settings - Invalid setting exists"
        );
    }

    // Get valid settings
    [UnityTest]
    public IEnumerator Test_SM2_1()
    {
        yield return null;
        Assert.IsTrue(
            themeOptions.Contains(PlayerPrefs.GetString("SETTING_THEME")),
            "Settings - Theme not being stored properly"
        );
        Assert.IsTrue(
            localizationOptions.Contains(PlayerPrefs.GetString("SETTING_LANGUAGE")),
            "Settings - Language not being stored properly"
        );
    }
}
