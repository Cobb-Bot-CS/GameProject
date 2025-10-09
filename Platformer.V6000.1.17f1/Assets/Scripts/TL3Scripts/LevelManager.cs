using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject player;

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.Find("CharacterParent");
        }
    }

    // maybe levels 1xx in first era, 2xx in second era, etc.
    // accepts positive integers
    // nextLevel is the level player is going to
    // currentLevel is the level player is leaving
    public void LoadLevel(int nextLevel, int currentLevel)
    {
        switch (nextLevel)
        {
            case 100:
                SceneManager.LoadScene("SceneV1");
                break;

            case 101:
                SceneManager.LoadScene("Level101");
                // place player in appropriate position
                if (player == null)
                {
                    if (currentLevel == 100)
                    {
                        player.transform.position = new Vector3(0.5f, 0.65f, 0.0f);
                    }
                    else
                    {
                        player.transform.position = new Vector3(3.0f, 0.65f, 0.0f);
                    }
                }
                break;

            case 102:
                SceneManager.LoadScene("Level102");
                break;

            default:
                SceneManager.LoadScene("MainMenu");
                break;
        }
    }
}
