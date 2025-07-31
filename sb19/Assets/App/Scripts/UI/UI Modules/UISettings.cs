using UnityEngine;
using UnityEngine.UIElements;

public class UISettings : MonoBehaviour
{

    private VisualElement SettingButton;
    private VisualElement SettingsPanel;
    public void Initialize(UIDocument doc)
    {
        SettingButton = doc.rootVisualElement.Q<VisualElement>("Settings-button");
        SettingsPanel = doc.rootVisualElement.Q<VisualElement>("Settings");
        SettingButton.RegisterCallback<ClickEvent>(OnButtonSettingsClicked);
    }

    public void OnButtonSettingsClicked(ClickEvent evt)
    {
        if (SettingsPanel.ClassListContains("settings-panel-in")) SettingsPanel.RemoveFromClassList("settings-panel-in");
        else SettingsPanel.AddToClassList("settings-panel-in");
        Debug.LogWarning("CLICKED!");
    }
}
