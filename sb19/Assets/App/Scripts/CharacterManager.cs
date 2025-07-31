
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;
    //[this is where I access controls]
    public CharacterController character; //for animation
    public NeedsController needsController;
   // public BackgroundController backgroundController;
    public CharacterEvolution evolutionController;
    public CharacterRandomizer randomizerController;

    public RoomManager roomController;

    //public Transform outfittingPosition;
    public Transform[] waypoints;
    public Transform OutfittingPosition;
    public float petMoveTimer, originalPetMoveTimer;

    [SerializeField]
    public string name;
    //toComment
    [Header("All Buddy ScriptableObjects")]
    public List<Character_SO> buddies;

    private Dictionary<string, Character_SO> buddyDictionary = new Dictionary<string, Character_SO>();

    private bool isTapped = false;
    public bool isOutfitting = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            addBuddyToDictionary();
            character.Idle();
            
            originalPetMoveTimer = petMoveTimer;
        }
    }

    private void Update()
    {
        if (petMoveTimer > 0)
        {
            petMoveTimer -= Time.deltaTime;
        }
        else
        {
            MovePetRandomWayPoint();
            petMoveTimer = originalPetMoveTimer;
        }
    }
    private void MovePetRandomWayPoint()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned for pet movement.");
            return;
        }
        if (isOutfitting)
        {
            //
            Debug.LogWarning("POSITIONG OUTFIT");
           // character.transform.position = OutfittingPosition.position;
            character.Move(OutfittingPosition.position);
            character.Idle();
            return;
        }

        if (needsController.energy <= 50 && needsController.food <= 50)
        {
            character.Move(waypoints[0].position);
            character.Idle();
            return;
        }
      

        if (needsController.isSleeping)
        {
            character.Move(waypoints[0].position);
            return;
        }
        else
        {
            int randomWayPoint = Random.Range(0, waypoints.Length);
            character.Move(waypoints[randomWayPoint].position);
            character.Walking();
        }  
    }

    public void addBuddyToDictionary()
    {
        foreach (var buddy in buddies)
        {
            if (!buddyDictionary.ContainsKey(buddy.buddyName)) buddyDictionary.Add(buddy.buddyName, buddy);
        }

    }

    private void Start()
    {
        RoomManager.Instance.ApplyAllActiveFurniture();
        //only set here
        string loadedName = FindExistingSavedBuddy();

        if (string.IsNullOrEmpty(loadedName)) 
        {
            string name = randomizerController.Randomizer().ToLower();
            SetCurrentBuddy(name);
            UIManager.Instance.TreeDisplay.SetTreeActive(); 
        } 
        else
        {
            SetCurrentBuddy(loadedName);
            UIManager.Instance.TreeDisplay.SetMainActive();
        }
    }

    
    public string FindExistingSavedBuddy()
    {
        foreach (var buddy in buddies)
        {
            string fileName = "character_" + buddy.buddyName.ToLower();
            if (DatabaseManager.instance.DoesCharacterFileExist(fileName)) return buddy.buddyName;
        }
        return null;
    }
    //toComment
    public void SetCurrentBuddy(string name)
    {
        DatabaseManager.instance.SetCurrentBuddyName(name);
    }

    public void lowFoodStatusState()
    {

    }

    public void Die()
    {
        Debug.Log("[CONSEQUENCE] DIE");
    }
}
