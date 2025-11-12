using UnityEngine;

public class GoalScript : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Player Went Through Wall And Made It To Goal");
    }
}
