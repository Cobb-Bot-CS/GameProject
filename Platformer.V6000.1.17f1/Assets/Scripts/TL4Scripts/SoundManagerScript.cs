using System.Collections;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    [SerializeField] private static SoundManagerScript I;
    [SerializeField] private AudioSource musicSource;

    void Awake()
    {
        if (I == null)
        {
            I = this;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void SetMusic(string musicId, float fadeTime, float targetVolume, bool loop)
    {
        StartCoroutine(FadeInMusic(fadeTime, targetVolume));
        musicSource.loop = loop;
    }
    
    private IEnumerator FadeInMusic(float duration, float targetVolume)
    {
        float startVolume = 0f;
        musicSource.volume = 0f;
        musicSource.Play();

        float timer = 0f;
        while (timer < duration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        musicSource.volume = targetVolume;
    }
}
