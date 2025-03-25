using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class AuthController : MonoBehaviour
{

    public AuthorizedClient client;

    public UIDocument signInPage;
    public UIDocument signUpPage;

    private VisualElement signInUI;
    private VisualElement signUpUI;

    private TextField usernameSI;
    private TextField passwordSI;
    private TextField nameField;
    private TextField usernameSU;
    private TextField passwordSU;
    private Button signIn;
    private Button signUp;
    private Button createNewAccount;
    private Button returnToSignIn;

    private DropdownField typeSU;

    private void Awake()
    {
        signInUI = signInPage.rootVisualElement;
        signUpUI = signUpPage.rootVisualElement;

        signUpUI.style.display = DisplayStyle.None;
        signInUI.style.display = DisplayStyle.Flex;
    }

    private void OnEnable()
    {
        usernameSI = signInUI.Q<TextField>("Username");
        passwordSI = signInUI.Q<TextField>("Password");
        nameField = signUpUI.Q<TextField>("Name");
        usernameSU = signUpUI.Q<TextField>("Username");
        passwordSU = signUpUI.Q<TextField>("Password");

        typeSU = signUpUI.Q<DropdownField>("Type");

        passwordSI.isPasswordField = true;

        usernameSI.textEdition.hidePlaceholderOnFocus = true;
        passwordSI.textEdition.hidePlaceholderOnFocus = true;
        nameField.textEdition.hidePlaceholderOnFocus = true;
        usernameSU.textEdition.hidePlaceholderOnFocus = true;
        passwordSU.textEdition.hidePlaceholderOnFocus = true;

        signIn = signInUI.Q<Button>("SignIn");
        signUp = signUpUI.Q<Button>("SignUp");
        createNewAccount = signInUI.Q<Button>("CreateNewAccount");
        returnToSignIn = signUpUI.Q<Button>("ReturnToSignIn");

        signIn.clicked += handleSignIn;
        signUp.clicked += handleSignUp;
        createNewAccount.clicked += goToSignUp;
        returnToSignIn.clicked += goToSignIn;
    }

    private async void handleSignIn()
    {
        await client.authUser(usernameSI.text, passwordSI.text);
        if (client.pb.AuthStore.IsValid())
            goToMainScreen();
    }

    private void handleSignUp()
    {
        client.createNewUser(nameField.text, usernameSU.text, passwordSU.text, typeSU.value);
        goToMainScreen();
    }

    private void goToMainScreen()
    {
        SceneManager.LoadScene(1);
    }

    private void goToSignUp()
    {
        usernameSI.SetValueWithoutNotify("");
        passwordSI.SetValueWithoutNotify("");
        signInUI.style.display = DisplayStyle.None;
        signUpUI.style.display = DisplayStyle.Flex;
    }

    private void goToSignIn()
    {
        nameField.SetValueWithoutNotify("");
        usernameSU.SetValueWithoutNotify("");
        passwordSU.SetValueWithoutNotify("");
        signUpUI.style.display = DisplayStyle.None;
        signInUI.style.display = DisplayStyle.Flex;
    }
}
