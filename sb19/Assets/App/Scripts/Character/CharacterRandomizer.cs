using UnityEngine;

public class CharacterRandomizer : MonoBehaviour
{

    public string Randomizer()
    {
        string[] names = { "PABLO", "JOSH", "KEN", "JUSTIN", "STELL" };
        int index = Random.Range(0, names.Length); // Unity's Random
        return names[index];
    }
}
