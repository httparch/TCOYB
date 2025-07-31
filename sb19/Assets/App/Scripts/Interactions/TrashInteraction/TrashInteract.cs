using UnityEngine;

public class TrashInteract : MonoBehaviour
{
    public int rewardAmount = 10;

    private void OnMouseDown()
    {
        Debug.Log("Mess clicked! Adding gold.");
        CharacterManager.instance.needsController.ChangeMoney(rewardAmount);

        // Optional: play sound or animation here

        Destroy(gameObject);
    }
}
