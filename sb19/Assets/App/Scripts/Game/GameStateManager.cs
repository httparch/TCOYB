using UnityEngine;



public enum GameState { MainUI, SelectionUI }
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public GameObject mainUI;
    public GameObject selectionUI;

    private GameState currentState;

    int flag = 0;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SetGameState(GameState newState)
    {
        currentState = newState;

        mainUI.SetActive(newState == GameState.MainUI);
  
        selectionUI.SetActive(newState == GameState.SelectionUI);
    }

    public GameState GetCurrentState() => currentState;
}
