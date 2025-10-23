using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class EnemyMoveScript : MonoBehaviour
{
    private float moveDistance = 2f;
    [SerializeField] private float moveSpeed = 2f;

    private Vector2 startPosition;
    private Vector2 targetPosition;
    private bool movingUp;
    
    void Start()
    {
        GetComponent<Rigidbody2D>();
        float lowerBound = startPosition.y - 2f;
        float upperBound = startPosition.y + 2f;

    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if( Vector2.Distance(transform.position, targetPosition)< 0.01f)
        {
            movingUp = !movingUp;

            if (movingUp)
            {
                targetPosition = startPosition + Vector2.up * moveDistance;
            }
            else
            {
               targetPosition = startPosition + Vector2.down * moveDistance;  
            }
        }
    }
}
