using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single sound with its properties
/// </summary>
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop = false;
}

/// <summary>
/// Singleton AudioManager for handling music and sound effects
/// </summary>
public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        InitializeSingleton();
        BuildSoundDictionary();
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Inspector Fields
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Library")]
    [SerializeField] private Sound[] soundEffects;
    
    [Header("Audio Settings")]
    [SerializeField] [Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;
    #endregion

    #region Private Fields
    private Dictionary<string, Sound> soundDictionary;
    private Coroutine currentFadeCoroutine;
    #endregion

    #region Initialization
    private void BuildSoundDictionary()
    {
        soundDictionary = new Dictionary<string, Sound>();
        
        foreach (Sound sound in soundEffects)
        {
            if (string.IsNullOrEmpty(sound.name))
            {
                Debug.LogWarning("⚠️ Sound with no name found - skipping");
                continue;
            }

            if (sound.clip == null)
            {
                Debug.LogWarning($"⚠️ Sound '{sound.name}' has no audio clip assigned");
                continue;
            }

            if (!soundDictionary.ContainsKey(sound.name))
            {
                soundDictionary.Add(sound.name, sound);
            }
            else
            {
                Debug.LogWarning($"⚠️ Duplicate sound name: {sound.name}");
            }
        }
        
        Debug.Log($"✓ Loaded {soundDictionary.Count} sounds");
    }
    #endregion

    #region Sound Effects Playback
    /// <summary>
    /// Plays a sound effect by name
    /// </summary>
    public void PlaySFX(string soundName)
    {
        if (TryGetSound(soundName, out Sound sound))
        {
            sfxSource.pitch = sound.pitch;
            sfxSource.PlayOneShot(sound.clip, sound.volume * sfxVolume * masterVolume);
        }
    }

    /// <summary>
    /// Plays a sound effect with random pitch variation
    /// </summary>
    public void PlaySFXWithRandomPitch(string soundName, float minPitch = 0.9f, float maxPitch = 1.1f)
    {
        if (TryGetSound(soundName, out Sound sound))
        {
            sfxSource.pitch = Random.Range(minPitch, maxPitch);
            sfxSource.PlayOneShot(sound.clip, sound.volume * sfxVolume * masterVolume);
            sfxSource.pitch = 1f;
        }
    }

    /// <summary>
    /// Plays a sound effect at a specific 3D position
    /// </summary>
    public void PlaySFXAtPosition(string soundName, Vector3 position)
    {
        if (TryGetSound(soundName, out Sound sound))
        {
            AudioSource.PlayClipAtPoint(sound.clip, position, sound.volume * sfxVolume * masterVolume);
        }
    }

    /// <summary>
    /// Stops all currently playing sound effects
    /// </summary>
    public void StopAllSFX()
    {
        sfxSource.Stop();
    }
    #endregion

    #region Music Playback
    /// <summary>
    /// Plays music with fade-in effect
    /// </summary>
    public void PlayMusic(string soundName, float fadeTime = 1f, bool loop = true)
    {
        if (TryGetSound(soundName, out Sound sound))
        {
            PlayMusic(sound.clip, fadeTime, sound.volume, loop);
        }
    }

    /// <summary>
    /// Plays music clip directly with fade-in
    /// </summary>
    public void PlayMusic(AudioClip music, float fadeTime = 1f, float targetVolume = 1f, bool loop = true)
    {
        if (music == null)
        {
            Debug.LogWarning("❌ Cannot play null music clip");
            return;
        }

        StopCurrentFade();
        currentFadeCoroutine = StartCoroutine(FadeInMusicCoroutine(music, fadeTime, targetVolume, loop));
    }

    /// <summary>
    /// Stops music with fade-out effect
    /// </summary>
    public void StopMusic(float fadeTime = 1f)
    {
        StopCurrentFade();
        currentFadeCoroutine = StartCoroutine(FadeOutMusicCoroutine(fadeTime));
    }

    /// <summary>
    /// Pauses the current music
    /// </summary>
    public void PauseMusic()
    {
        musicSource.Pause();
    }

    /// <summary>
    /// Resumes paused music
    /// </summary>
    public void ResumeMusic()
    {
        musicSource.UnPause();
    }
    #endregion

    #region Volume Controls
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    private void UpdateVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
    }
    #endregion

    #region Helper Methods
    private bool TryGetSound(string soundName, out Sound sound)
    {
        if (soundDictionary.TryGetValue(soundName, out sound))
        {
            return true;
        }
        
        Debug.LogWarning($"❌ Sound not found: {soundName}");
        return false;
    }

    private void StopCurrentFade()
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = null;
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator FadeInMusicCoroutine(AudioClip music, float duration, float targetVolume, bool loop)
    {
        musicSource.clip = music;
        musicSource.loop = loop;
        musicSource.volume = 0f;
        musicSource.Play();

        float elapsed = 0f;
        float finalVolume = targetVolume * musicVolume * masterVolume;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, finalVolume, elapsed / duration);
            yield return null;
        }

        musicSource.volume = finalVolume;
        currentFadeCoroutine = null;
    }

    private IEnumerator FadeOutMusicCoroutine(float duration)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;
        currentFadeCoroutine = null;
    }
    #endregion

    #region Debug
    [ContextMenu("List All Sounds")]
    private void ListAllSounds()
    {
        Debug.Log("=== Sound Library ===");
        foreach (var kvp in soundDictionary)
        {
            Debug.Log($"• {kvp.Key}: {kvp.Value.clip.name}");
        }
    }
    #endregion
}