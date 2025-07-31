using UnityEngine;
using UnityEngine.UIElements;

public class UICharacterBag : MonoBehaviour
{
    public VisualElement bagButton;
    public VisualElement characterListContainer;
    public VisualElement CharacterButton;
    public void Initialize(UIDocument docs)
    {
        bagButton = docs.rootVisualElement.Q<VisualElement>("BagButton");
        characterListContainer = docs.rootVisualElement.Q<VisualElement>("CharacterListContainer");
        CharacterButton = docs.rootVisualElement.Q<VisualElement>("CharacterButton"); // to be tempalted 
        bagButton.RegisterCallback<ClickEvent>(OnBagClicked);
    }

    private void OnBagClicked(ClickEvent evt)
    {
        if (characterListContainer.ClassListContains("CLC-in")) characterListContainer.RemoveFromClassList("CLC-in");
        else characterListContainer.AddToClassList("CLC-in");
    }
}
