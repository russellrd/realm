using System.Collections;
using NUnit.Framework;
using Realm;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class WeatherHazardTest
{
    [UnityTest]
    public IEnumerator Test_WHDM1()
    {
        LogAssert.ignoreFailingMessages = true;
        SceneManager.LoadScene("Assets/Scenes/RealmInterface.unity", LoadSceneMode.Single);
        while (SceneManager.GetActiveScene().buildIndex > 0)
            yield return null;

        GPSCoordinate torontoCoordinate = new(43.651070, -79.347015);
        WeatherDTO weather = new();

        using UnityWebRequest www = UnityWebRequest.Get($"https://api.open-meteo.com/v1/forecast?latitude={torontoCoordinate.Latitude}&longitude={torontoCoordinate.Longitude}&hourly=weather_code,temperature_2m,precipitation,precipitation_probability&current=temperature_2m,weather_code,cloud_cover,precipitation&timezone=auto");
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            weather = WeatherDTO.FromJSON(www.downloadHandler.text);
        }
        else
        {
            Assert.Fail("WeatherHazard - Could not make weather request");
        }

        var realWorldControllerObject = GameObject.Find("RealWorld Controller").gameObject;
        var realWorldController = realWorldControllerObject.GetComponent<RealWorldController>();
        var code = realWorldController.CheckForWeatherHazards();

        Assert.AreEqual(code, weather.Current.WeatherCode, "WeatherHazard - Weather code does not match for Toronto");
    }
}
