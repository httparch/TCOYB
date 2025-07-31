using UnityEngine;

public class MoodManager : MonoBehaviour
{
    public float gameHourTimer;
    public float hourLength;

    public static MoodManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void Update()
    {
        if(gameHourTimer <= 0) gameHourTimer = hourLength;
        else gameHourTimer -= Time.deltaTime;
    }
        
}
