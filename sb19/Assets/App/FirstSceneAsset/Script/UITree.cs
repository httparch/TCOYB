
using UnityEngine;
using UnityEngine.UIElements;

public class UITree : MonoBehaviour
{ 

    private VisualElement Apple1;
    private VisualElement Apple2;
    private VisualElement Apple3;
    private VisualElement Apple4;
    private VisualElement Apple5;

    private VisualElement DailyRewardButton, HideDisplayButton, Outfitter;

    public GameObject Main;
    public GameObject Tree;

    public VisualElement App;
    private VisualElement TreeContainer;
    public static bool HasPickedCharacter = false;

    private Label Countdown;
    public void Initialize(UIDocument UI)
    {

        App = UI.rootVisualElement.Q<VisualElement>("App");
        TreeContainer = UI.rootVisualElement.Q<VisualElement>("TreeContainer");
        DailyRewardButton = UI.rootVisualElement.Q<VisualElement>("DailyRewardButton");
        HideDisplayButton = UI.rootVisualElement.Q<VisualElement>("HideDisplayButton");
        Outfitter = UI.rootVisualElement.Q<VisualElement>("Outfitter");

        Apple1 = UI.rootVisualElement.Q<VisualElement>("Apple1");
        Apple2 = UI.rootVisualElement.Q<VisualElement>("Apple2");
        Apple3 = UI.rootVisualElement.Q<VisualElement>("Apple3");
        Apple4 = UI.rootVisualElement.Q<VisualElement>("Apple4");
        Apple5 = UI.rootVisualElement.Q<VisualElement>("Apple5");

        Countdown = UI.rootVisualElement.Q<Label>("Countdown");

        Apple1.RegisterCallback<ClickEvent>(OnAppleClicked);
        Apple2.RegisterCallback<ClickEvent>(OnAppleClicked);
        Apple3.RegisterCallback<ClickEvent>(OnAppleClicked);
        Apple4.RegisterCallback<ClickEvent>(OnAppleClicked);
        Apple5.RegisterCallback<ClickEvent>(OnAppleClicked);


    }

    public void SetTreeActive()
    {
        TreeContainer.style.display = DisplayStyle.Flex;
        Tree.SetActive(true);

        App.style.display = DisplayStyle.None;
        DailyRewardButton.style.display = DisplayStyle.None;
        HideDisplayButton.style.display = DisplayStyle.None;
        Outfitter.style.display = DisplayStyle.None;
        Main.SetActive(false);
    }
    public void SetMainActive()
    {
        TreeContainer.style.display = DisplayStyle.None;
        Tree.SetActive(false);

        App.style.display = DisplayStyle.Flex;
        DailyRewardButton.style.display = DisplayStyle.Flex;
        HideDisplayButton.style.display = DisplayStyle.Flex;
        Outfitter.style.display = DisplayStyle.Flex;
        Main.SetActive(true);
    }

    public bool IsMainActive()
    {
        return App.style.display == DisplayStyle.Flex;
    }
    private void OnAppleClicked(ClickEvent evt)
    {
        HasPickedCharacter = true;
        SetMainActive();
    }

}