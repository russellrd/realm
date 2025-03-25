using UnityEngine;
using PocketBaseSdk;
using UnityEngine.SceneManagement;
public class DatabaseController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PocketBase pb;

    private void Start()
    {
        pb = new PocketBase("https://pocketbase.midnightstudio.me", "en-US", AsyncAuthStore.PlayerPrefs);
        if (pb.AuthStore == null || !pb.AuthStore.IsValid())
        {
            SceneManager.LoadScene(0);
        }
    }

    public void logout()
    {
        pb.AuthStore.Clear();
        SceneManager.LoadScene(0);
    }
}
