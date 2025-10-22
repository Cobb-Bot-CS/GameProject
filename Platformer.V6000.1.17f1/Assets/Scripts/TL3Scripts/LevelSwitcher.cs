using UnityEngine;

public class LevelSwitcher : MonoBehaviour
{
    [SerializeField] private int nextLevel;
    [SerializeField] private int currentLevel;

    [SerializeField] private GameObject levelManagerObject;
    private LevelManager levelManager;

    private void Awake()
    {
        // Make sure we have level manager gameobject
        if (levelManagerObject == null)
        {
            levelManagerObject = GameObject.Find("LevelManager");
        }
        // get the level manager script to use methods
        if (levelManagerObject != null)
        {
            levelManager = levelManagerObject.GetComponent<LevelManager>();
        }
    }

    // upon colliding with player switch the level
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided");
        if (levelManager != null)
        {
            GameObject otherGameObject = other.gameObject;
            if (otherGameObject.CompareTag("Player"))
            {
                levelManager.LoadLevel(nextLevel, currentLevel);
            }
        }
        else
        {
            Debug.Log("Error: Couldn't find level manager");
        }
    }
}
