using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Animator characterAnimator;
    private Vector3 destination;
    public float speed;

    private void Update()
    {
        if(Vector3.Distance(transform.position, destination) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        }
    }

    public void Move(Vector3 destination)
    {
        this.destination = destination;
    }
    public void Happy()
    {
        //animator use setTrigger
       // characterAnimator.SetTrigger("Character_happy");
    }

    public void Hungry()
    {
        // characterAnimator.SetTrigger("Character_hungry");
    }

    public void Sad()
    {
        //characterAnimator.SetTrigger("Character_sad");
    }

    public void Tired()
    {
       // characterAnimator.SetTrigger("Character_tired");
    }

    public void Eat()
    {
       // characterAnimator.SetTrigger("Character_eat");
    }

    public void Walking()
    {
        characterAnimator.Play("WALKING");
    }
    public void Idle()
    {
        characterAnimator.Play("IDLE");
        //characterAnimator.SetTrigger("Character_idle");
    }
 
}
