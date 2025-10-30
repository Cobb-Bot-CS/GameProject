using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

/*
 * Filename: EnemyMoveScript.cs
 * Developer: Aj Karki
 * Purpose: Moves the enemy up and down between two points.
 */

/// <summary>
/// Handles simple vertical patrol movement for an enemy.
/// The enemy moves between a lower and upper point using MoveTowards.
/// </summary>
public class EnemyMoveScript : MonoBehaviour
{
    // How far (in units) the enemy moves up and down from the start position
    [SerializeField] private float moveDistance = 2f;

    // How fast the enemy moves
    [SerializeField] private float moveSpeed = 2f;

    // Starting position of the enemy when the scene begins
    private Vector2 startPosition;

    // Current target position the enemy is moving toward
    private Vector2 targetPosition;

    // Whether the enemy is currently moving upward
    private bool movingUp;


    /// <summary>
    /// Initializes the movement positions and sets the first target.
    /// </summary>
    private void Start()
    {
        // Record the position where the enemy started
        startPosition = transform.position;

        // Start by moving upward
        movingUp = true;
        targetPosition = startPosition + Vector2.up * moveDistance;

        // If you need physics later, keep this; otherwise remove
        GetComponent<Rigidbody2D>();

    }


    /// <summary>
    /// Moves the enemy toward the current target and flips the direction
    /// when the target is reached.
    /// </summary>
    private void Update()
    {
        // Move toward the current target
        transform.position = Vector2.MoveTowards(
           transform.position,
           targetPosition,
           moveSpeed * Time.deltaTime
        );

        // If close enough to the target, switch direction
        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
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
