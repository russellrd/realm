using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using PocketBaseSdk;
using Realm;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class AuthenticationTest
{
    [UnitySetUp]
    public IEnumerable Setup()
    {
        LogAssert.ignoreFailingMessages = true;
        SceneManager.LoadScene("Assets/Scenes/RealmInterface.unity", LoadSceneMode.Single);
        while (SceneManager.GetActiveScene().buildIndex > 0)
            yield return null;

        if (DatabaseController.IsLoggedIn())
            DatabaseController.Logout();
    }

    // General User Login
    [UnityTest]
    public IEnumerator Test_AM1()
    {
        Task<RecordAuth> task = AuthGeneralUser();

        while (!task.IsCompleted)
        {
            yield return null;
        }

        Assert.True(task.Result.Record.Id == "txp0861fk49icm0", "Authentication - User does not match auth");
        Assert.True(DatabaseController.GetCurrentUserType() == "general", "Authentication - User should be general type");
        Assert.True(DatabaseController.IsValidUser(), "Authentication - User not valid");
    }

    private async Task<RecordAuth> AuthGeneralUser()
    {
        return await DatabaseController.AuthenticateUser("g@test.com", "12345678");
    }

    // Organization User Login
    [UnityTest]
    public IEnumerator Test_AM2()
    {
        Task<RecordAuth> task = AuthOrganizationUser();

        while (!task.IsCompleted)
        {
            yield return null;
        }

        Assert.True(task.Result.Record.Id == "kpp6357f7re9a1o", "Authentication - User does not match auth");
        Assert.True(DatabaseController.GetCurrentUserType() == "organization", "Authentication - User should be organization type");
        Assert.True(DatabaseController.GetCurrentUserOrganizationId() == "ee8krgxk61jt6f9", "Authentication - User has wrong organization");
        Assert.True(DatabaseController.IsValidUser(), "Authentication - User not valid");
    }

    private async Task<RecordAuth> AuthOrganizationUser()
    {
        return await DatabaseController.AuthenticateUser("o@test.com", "12345678");
    }

    // Logout
    [UnityTest]
    public IEnumerator Test_AM3()
    {
        yield return null;
        Assert.False(DatabaseController.IsValidUser(), "Authentication - Logout failed");
    }
}
