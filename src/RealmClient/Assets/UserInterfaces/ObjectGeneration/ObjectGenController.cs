using GLTFast;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectGenController : MonoBehaviour
{
    public Camera previewCamera;
    public UIDocument mainPage;
    public UIDocument promptScreen;
    public UIDocument previewScreen;

    private VisualElement mainUI;
    private VisualElement promptUI;
    private VisualElement previewUI;

    private TextField creationName;
    private TextField creationPrompt;
    private Button promptGenerate;
    private Button plusButton;
    private Button mainScreenBackButton;
    private Button promptScreenBackButton;

    private Label emptyText;
    private VisualElement listItem;
    private Button previewButton;
    private Button previewScreenBackButton;

    private GameObject objectToPreview;



    private GltfImport generatedObject;

    private void Awake()
    {
        mainUI = mainPage.rootVisualElement;
        promptUI = promptScreen.rootVisualElement;
        previewUI = previewScreen.rootVisualElement;

        promptUI.style.display = DisplayStyle.None;
        previewUI.style.display = DisplayStyle.None;
        mainUI.style.display = DisplayStyle.Flex;

        objectToPreview = new();
    }

    private void OnEnable()
    {
        creationName = promptUI.Q<TextField>("Name");
        creationPrompt = promptUI.Q<TextField>("Prompt");

        creationPrompt.style.whiteSpace = WhiteSpace.Normal;

        creationName.textEdition.hidePlaceholderOnFocus = true;
        creationPrompt.textEdition.hidePlaceholderOnFocus = true;

        promptGenerate = promptUI.Q<Button>("GenerationButton");
        plusButton = mainUI.Q<Button>("PlusButton");

        mainScreenBackButton = mainUI.Q<Button>("BackButton");
        promptScreenBackButton = promptUI.Q<Button>("BackButton");

        promptGenerate.clicked += sendGenReq;
        plusButton.clicked += goToPromptScreen;

        mainScreenBackButton.clicked += goToRootScreen;
        promptScreenBackButton.clicked += goToMainScreen;

        emptyText = mainUI.Q<Label>("EmptyText");
        listItem = mainUI.Q<VisualElement>("Entry");
        previewButton = mainUI.Q<Button>("InstancePreview");
        previewScreenBackButton = previewUI.Q<Button>();

        previewButton.clicked += previewObject;
        previewScreenBackButton.clicked += endPreview;

        listItem.style.display = DisplayStyle.None;

    }

    private async void sendGenReq()
    {
        emptyText.style.display = DisplayStyle.None;
        string name = creationName.text;
        ObjectGenRequestDTO data = new()
        {
            Text = creationPrompt.text,
            Texture = "true"
        };

        ObjectGenerationRequest req = new ObjectGenerationRequest(data, name);
        await req.promptGeneration();
        generatedObject = req.gltf;

        listItem.style.display = DisplayStyle.Flex;
        goToMainScreen();
    }

    private async void previewObject()
    {
        if (generatedObject != null)
        {
            creationName.SetValueWithoutNotify("");
            creationPrompt.SetValueWithoutNotify("");
            promptUI.style.display = DisplayStyle.None;
            mainUI.style.display = DisplayStyle.None;
            previewUI.style.display = DisplayStyle.Flex;

            objectToPreview.SetActive(true);
            await generatedObject.InstantiateMainSceneAsync(objectToPreview.transform);
            previewCamera.transform.position = objectToPreview.transform.position + new Vector3 { x = 0, y = 5, z = -5 };

            previewCamera.transform.rotation = Quaternion.Euler(new Vector3 { x = 45, y = 0, z = 0 });
        }
        else
        {
            print("not yet");
        }

    }

    private void goToRootScreen()
    {
        // TODO
        // disable all screens and go back to realm screen prolly
    }

    private void endPreview()
    {
        objectToPreview.SetActive(false);
        goToMainScreen();
    }

    private void goToPromptScreen()
    {
        mainUI.style.display = DisplayStyle.None;
        promptUI.style.display = DisplayStyle.Flex;
    }

    private void goToMainScreen()
    {
        creationName.SetValueWithoutNotify("");
        creationPrompt.SetValueWithoutNotify("");
        promptUI.style.display = DisplayStyle.None;
        previewUI.style.display = DisplayStyle.None;
        mainUI.style.display = DisplayStyle.Flex;
    }
}
