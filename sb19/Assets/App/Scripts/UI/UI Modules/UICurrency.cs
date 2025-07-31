
using UnityEngine;
using UnityEngine.UIElements;

public class UICurrency : MonoBehaviour
{
    private Label Gold, Diamond;
    private Label CurrentGold;
    public void Initialize(UIDocument doc)
    {
        Gold = doc.rootVisualElement.Q<Label>("gold");
        Diamond = doc.rootVisualElement.Q<Label>("diamond");
        CurrentGold = doc.rootVisualElement.Q<Label>("CurrentGold");
    }
        
    public void UpdateCurrencyDisplay(int gold, int diamond)
    {
        
            Gold.text = gold.ToString();
            Diamond.text = diamond.ToString();
            CurrentGold.text = gold.ToString();

    }
}
