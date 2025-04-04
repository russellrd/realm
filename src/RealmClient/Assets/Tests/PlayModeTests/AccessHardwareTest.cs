using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class AccessHardwareTest
{
    private const int HEIGHT = 2280;
    private const int WIDTH = 1080;

    [UnitySetUp]
    public IEnumerable Setup()
    {
        LogAssert.ignoreFailingMessages = true;
        SceneManager.LoadScene("Assets/Scenes/RealmInterface.unity", LoadSceneMode.Single);
        yield return null;
        GameObject.Find("ARCore Extensions").SetActive(false);
    }

    // Height using Samsung Galaxy S10e simulator
    [UnityTest]
    public IEnumerator Test_AHM1()
    {
        yield return null;
        Assert.AreEqual(Screen.height, HEIGHT, "AccessHardware - Height not equal");
    }

    // Width using Samsung Galaxy S10e simulator
    [UnityTest]
    public IEnumerator Test_AHM2()
    {
        yield return null;
        Assert.AreEqual(Screen.width, WIDTH, "AccessHardware - Width not equal");
    }
}
