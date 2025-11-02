using UnityEngine;

public class PlayerDemoController : MonoBehaviour
{
    public Rigidbody2D rb;              // drag Character here
    public float moveSpeed = 3f;
    public Transform[] waypoints;       // drag DemoPoint1, DemoPoint2, ...

    private int currentIndex = 0;
    private bool isRunning = false;

    public void StartDemo()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("Demo: no waypoints set!");
            return;
        }

        currentIndex = 0;
        isRunning = true;
        Debug.Log("Demo: START");
    }

    public void StopDemo()
    {
        isRunning = false;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        Debug.Log("Demo: STOP");
    }

    void Update()
    {
        if (!isRunning) return;
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];

        // direction to target
        Vector2 pos = rb.position;
        Vector2 dir = (target.position - transform.position);
        Vector2 step = dir.normalized * moveSpeed;

        // move
        rb.linearVelocity = new Vector2(step.x, rb.linearVelocity.y);

        // reached?
        if (Vector2.Distance(transform.position, target.position) < 0.15f)
        {
            currentIndex++;

            // done with all points
            if (currentIndex >= waypoints.Length)
            {
                StopDemo();
            }
        }
    }
}
