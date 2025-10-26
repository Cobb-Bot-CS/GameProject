using UnityEngine;

public class SoundScript : MonoBehaviour
{
    [SerializeField] private SoundManagerScript soundManager;
    [SerializeField] private string musicId = "bgm_menu";
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private float targetVolume = 0.8f;

    void Start()
    {
        if (soundManager != null)
        {
            soundManager.SetMusic(musicId, fadeTime, targetVolume, true);
        }
    }
}
