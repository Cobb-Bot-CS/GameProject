using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player touched the win circle!");

            // Show the WinScreen
            WinScreen winScreen = FindAnyObjectByType<WinScreen>();
            if (winScreen != null)
                winScreen.ShowWinScreen();
            else
                Debug.LogError("WinScreen not found in scene!");
        }
    }
}
