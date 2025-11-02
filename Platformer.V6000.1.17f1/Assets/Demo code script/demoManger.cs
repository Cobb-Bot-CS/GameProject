using UnityEngine;

public class DemoManager : MonoBehaviour
{
    public PlayerInputController inputController;
    public PlayerDemoController demoController;

    public float idleTimeToStartDemo = 5f;
    private float idleTimer = 0f;
    private bool inDemo = false;

    void Update()
    {
        // only count REAL input (key/mouse/button)
        bool userInput = Input.anyKeyDown;

        if (userInput)
        {
            // If player interacts, stop demo and return control
            if (inDemo)
            {
                inDemo = false;
                demoController.StopDemo();
                inputController.EnableInput(true);
            }

            idleTimer = 0f;
            return;
        }

        // If no input, count idle time
        if (!inDemo)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTimeToStartDemo)
            {
                // Start demo after idle time
                inDemo = true;
                inputController.EnableInput(false);
                demoController.StartDemo();
                Debug.Log("DEMO STARTED ✅");
            }
        }
    }
}
