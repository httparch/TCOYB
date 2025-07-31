using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ScreenShotManager : MonoBehaviour
{

    private VisualElement CameraButton;
    public void Initialize(UIDocument docs)
    {
        CameraButton = docs.rootVisualElement.Q<VisualElement>("Camera-button");

        CameraButton.RegisterCallback<ClickEvent>(OnCameraClicked);
    }

    private void OnCameraClicked(ClickEvent evt)
    {
        Debug.Log("CAMERA CLICKED!");
        //[todo someday] deactivate UI,
        ScreenCapture.CaptureScreenshot("screenshot-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png");
        //screenshot will be seen in root folder
    }
}
