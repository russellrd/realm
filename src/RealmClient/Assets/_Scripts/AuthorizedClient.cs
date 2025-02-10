using UnityEngine;
using PocketBaseSdk;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class AuthorizedClient : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PocketBase pb;

    private void Start()
    {
        pb = new PocketBase("https://pocketbase.midnightstudio.me", "en-US", AsyncAuthStore.PlayerPrefs);
        if (pb.AuthStore != null && pb.AuthStore.IsValid())
        {
            Debug.Log("REALM: logged-in");
            SceneManager.LoadScene(1);
            Debug.Log(pb.AuthStore.ToString());
        }
    }

    public async Task<RecordAuth> authUser(string email, string password)
    {
        var userData = await pb.Collection("users").AuthWithPassword(email, password);
        Debug.Log("REALM: User " + userData.Record.Email);
        // List<RecordModel> test = await pb.Collection("testing").GetFullList();
        // print((string)test.First()["name"]);
        return userData;
    }

    public async void createNewUser(string name, string email, string password)
    {
        UserCreateDTO data = new()
        {
            Name = name,
            Email = email,
            Password = password,
            PasswordConfirm = password
        };

        try
        {
            var record = await pb.Collection("users").Create(data);
        }
        catch (Exception e)
        {
            print(e.InnerException.ToString());
        }

        await authUser(email, password);
    }
}
