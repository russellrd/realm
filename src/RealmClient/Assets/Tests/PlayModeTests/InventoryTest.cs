using System.Collections;
using NUnit.Framework;
using Realm;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class InventoryTest
{
    [UnitySetUp]
    public IEnumerable Setup()
    {
        LogAssert.ignoreFailingMessages = true;
        SceneManager.LoadScene("Assets/Scenes/RealmInterface.unity", LoadSceneMode.Single);
        yield return null;
        GameObject.Find("ARCore Extensions").SetActive(false);
    }

    // Objects in inventory less than MAX_OBJ_COUNT
    [UnityTest]
    public IEnumerator Test_IM1()
    {
        LogAssert.ignoreFailingMessages = true;
        SceneManager.LoadScene("Assets/Scenes/RealmInterface.unity", LoadSceneMode.Single);
        while (SceneManager.GetActiveScene().buildIndex > 0)
            yield return null;

        var contentObject = GameObject.Find("UI").gameObject.transform
                                    .Find("Inventory").gameObject.transform
                                    .Find("ObjectsToSelect").gameObject.transform
                                    .Find("Scroll View").gameObject.transform
                                    .Find("Viewport").gameObject.transform
                                    .Find("Content");
        Debug.Log(contentObject.transform.childCount);
        Assert.LessOrEqual(contentObject.transform.childCount, InventoryManager.MAX_OBJ_COUNT);

        var modelStoreObject = GameObject.Find("ModelStore").gameObject;
        Assert.LessOrEqual(modelStoreObject.transform.childCount, InventoryManager.MAX_OBJ_COUNT);
    }
}
