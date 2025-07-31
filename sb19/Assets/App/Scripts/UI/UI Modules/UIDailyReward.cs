using UnityEngine;
using UnityEngine.UIElements;

public class UIDailyReward : MonoBehaviour
{
    public VisualTreeAsset dailyRewardTemplate; 
    public DailyReward rewardSystem;

    private VisualElement DailyRewardButton;
    private VisualElement DailyRewardPanel;
    private ScrollView DailyRewardScrollView;
    private VisualElement ExitButtonDR;
    public void Initialize(UIDocument doc)
    {
        DailyRewardButton = doc.rootVisualElement.Q<VisualElement>("DailyRewardButton");
        DailyRewardPanel = doc.rootVisualElement.Q<VisualElement>("DailyRewardPanel");
        ExitButtonDR = doc.rootVisualElement.Q<VisualElement>("ExitButtonDR");
        DailyRewardScrollView = doc.rootVisualElement.Q<ScrollView>("DailyRewardScrollView");

        DailyRewardButton.RegisterCallback<ClickEvent>(OnDailyRewardButtonClicked);
        ExitButtonDR.RegisterCallback<ClickEvent>(OnExitButtonClicked);

        PopulateDailyRewardsUI();
    }

    public void PopulateDailyRewardsUI()
    {
        DailyRewardScrollView.Clear();

        for (int i = 0; i < rewardSystem.rewards.Count; i++)
        {
            var reward = rewardSystem.rewards[i];
            VisualElement rewardBox = dailyRewardTemplate.Instantiate();
            /*
            switch (reward.state)
            {
                case RewardState.Claimed:
                    rewardBox.AddToClassList("claimed-reward");
                    break;
                case RewardState.Active:
                    rewardBox.AddToClassList("active-reward");
                    break;
                case RewardState.NotReady:
                    rewardBox.AddToClassList("notready-reward");
                    break;
            }*/

            // Set day number
            rewardBox.Q<Label>("DayNumber").text = $"Day {reward.dayNumber}";

            // Set reward name
            rewardBox.Q<Label>("RewardName").text = reward.rewardName;

            // Set reward image
            var rewardImageVE = rewardBox.Q<VisualElement>("RewardImage");
            if (reward.rewardImage != null)
            {
                rewardImageVE.style.backgroundImage = new StyleBackground(reward.rewardImage.texture);
            }

            // Handle button
            Button claimButton = rewardBox.Q<Button>("ClaimRewarButton");

            if (reward.state == RewardState.Claimed)
            {
                claimButton.text = "Claimed";
                claimButton.SetEnabled(false);
            }
            else if (reward.state == RewardState.NotReady)
            {
                claimButton.text = "Not Ready";
                claimButton.SetEnabled(false);
            }
            else if (reward.state == RewardState.Active)
            {
                claimButton.text = "Claim";
                claimButton.SetEnabled(true);

                int capturedIndex = i; // important for closures
                claimButton.clicked += () =>
                {
                    rewardSystem.ClaimReward();
                    PopulateDailyRewardsUI(); // Refresh UI after claim
                };
            }

            DailyRewardScrollView.Add(rewardBox);
        }
    }

    private void OnDailyRewardButtonClicked(ClickEvent evt)
    {
        DailyRewardPanel.style.display = DisplayStyle.Flex;
        Camera.main.GetComponent<CameraMovement>().LockCamera();
        Debug.LogWarning("CLICKED DR");
    }

    private void OnExitButtonClicked(ClickEvent evt)
    {
        DailyRewardPanel.style.display = DisplayStyle.None;
        Camera.main.GetComponent<CameraMovement>().UnlockCamera();
    }
}
