using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement("Popup")]
public partial class PopupCustomControl : VisualElement
{
    private const string STYLE_RESOURCE = "PopupStylesheet";
    private const string CLASS_POPUP = "popup";
    private const string CLASS_POPUP_ERROR = "popup_error";
    private const string CLASS_POPUP_CONTAINER = "popup_container";
    private const string CLASS_CONTAINER = "container";
    private const string CLASS_POPUP_TEXT = "popup_text";
    private const string CLASS_POPUP_BUTTON = "popup_button";
    private const string CLASS_BUTTON_PRIMARY = "button_primary";
    private const string CLASS_BUTTON_SECONDARY = "button_secondary";

    public event Action Primary;
    public event Action Secondary;

    private Label textLabel;
    private Button primaryButton;
    private Button secondaryButton;

    private VisualElement window;
    private VisualElement textContainer;
    private VisualElement buttonContainer;

    public PopupCustomControl()
    {
        Init();
    }

    public PopupCustomControl(bool isError = false, bool isMultiButton = false)
    {
        Init();

        if (isError)
        {
            window.AddToClassList(CLASS_POPUP_ERROR);
        }

        if (isMultiButton)
        {
            secondaryButton = new() { text = "CANCEL" };
            secondaryButton.AddToClassList(CLASS_BUTTON_SECONDARY);
            secondaryButton.AddToClassList(CLASS_POPUP_BUTTON);
            buttonContainer.Add(secondaryButton);

            secondaryButton.clicked += OnSecondaryClicked;
        }
    }

    public void SetText(string text)
    {
        textLabel.text = text;
    }

    public void SetPrimaryButtonText(string text)
    {
        primaryButton.text = text;
    }

    public void SetSecondaryButtonText(string text)
    {
        secondaryButton.text = text;
    }

    private void OnPrimaryClicked()
    {
        Primary?.Invoke();
    }

    private void OnSecondaryClicked()
    {
        Secondary?.Invoke();
    }

    private void Init()
    {
        styleSheets.Add(Resources.Load<StyleSheet>(STYLE_RESOURCE));
        AddToClassList(CLASS_POPUP_CONTAINER);

        window = new();
        window.AddToClassList(CLASS_POPUP);
        hierarchy.Add(window);

        textContainer = new();
        textContainer.AddToClassList(CLASS_CONTAINER);
        window.Add(textContainer);

        textLabel = new() { text = "THIS IS A TEST MESSAGE!" };
        textLabel.AddToClassList(CLASS_POPUP_TEXT);
        textContainer.Add(textLabel);

        buttonContainer = new();
        buttonContainer.AddToClassList(CLASS_CONTAINER);
        window.Add(buttonContainer);

        primaryButton = new() { text = "CONFIRM" };
        primaryButton.AddToClassList(CLASS_BUTTON_PRIMARY);
        primaryButton.AddToClassList(CLASS_POPUP_BUTTON);
        buttonContainer.Add(primaryButton);

        primaryButton.clicked += OnPrimaryClicked;
    }
}
