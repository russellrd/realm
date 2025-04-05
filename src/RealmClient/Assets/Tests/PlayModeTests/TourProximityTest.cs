using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using Realm;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TourProximityTest
{
    [UnityTest]
    public IEnumerator Test_TP1()
    {
        LogAssert.ignoreFailingMessages = true;
        SceneManager.LoadScene("Assets/Scenes/RealmInterface.unity", LoadSceneMode.Single);
        while (SceneManager.GetActiveScene().buildIndex > 0)
            yield return null;

        var tourCoords = new GPSCoordinate(43.26085870765585, -79.92001989636371);

        var realWorldControllerObject = GameObject.Find("RealWorld Controller").gameObject;

        var realWorldController = realWorldControllerObject.GetComponent<RealWorldController>();

        Task<TourDTO> task = realWorldController.IsTourInProximityAsync(tourCoords);

        while (!task.IsCompleted)
        {
            yield return null;
        }

        Assert.False(task.Result == null, "TourProximity - No tour found in proximity");

        Assert.True(task.Result.Id == "j6u60615l99bu9j", "TourProximity - Wrong tour found in proximity");
    }
}
