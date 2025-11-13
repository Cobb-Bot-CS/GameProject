using UnityEngine;

public class WinTrigger2D : MonoBehaviour
{
    private bool hasWon = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasWon) return; // Prevent multiple triggers

        Debug.Log("Trigger entered by: " + other.name);

        if (other.CompareTag("Player"))
        {
            hasWon = true;
            Debug.Log("🎉 Player reached the Win Circle!");

            // Find and show the Win Screen
            WinScreen winScreen = FindAnyObjectByType<WinScreen>();
            if (winScreen != null)
            {
                winScreen.ShowWinScreen();
            }
            else
            {
                Debug.LogWarning("⚠ No WinScreen found in the scene!");
            }
        }
    }
}
