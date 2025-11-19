using UnityEngine;

public class GoalScript : MonoBehaviour
{
    public bool reachedGoal = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            reachedGoal = true;
            Debug.Log("Player reached a goal!");
        }
}
}


