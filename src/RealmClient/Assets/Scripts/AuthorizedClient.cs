using UnityEngine;
using PocketBaseSdk;
using System;
using System.Collections.Generic;
using System.Linq;

public class AuthorizedClient : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PocketBase pb;

    private void Start()
    {
        pb = new PocketBase("https://pocketbase.midnightstudio.me");
    }

    public async void authUser(string email, string password)
    {
        var userData = await pb.Collection("users").AuthWithPassword(email, password);
        List<RecordModel> test = await pb.Collection("testing").GetFullList();
        print((string)test.First()["name"]);
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

        authUser(email, password);
    }
}
