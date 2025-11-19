using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Audio;

/*
 * filename: AudioManager.cs
 * Developer: Urvashi Gupta
 * Purpose: Manages all audio playback, volume settings, and sound initialization
 */
public class AudioManager : MonoBehaviour
{
   [SerializeField] private Slider volumeSlider; // Single master volume slider
   public static AudioManager Instance;
   public Sound[] sounds;

   [Header("Audio Mixer Groups")]
   [SerializeField] private AudioMixerGroup musicMixerGroup;
   [SerializeField] private AudioMixerGroup sfxMixerGroup;
   [SerializeField] private AudioMixerGroup uiMixerGroup;

   [Header("Audio Mixer")]
   [SerializeField] private AudioMixer audioMixer;

   private AudioScheduler scheduler;

   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
         DontDestroyOnLoad(gameObject);
      }
      else
      {
         Destroy(gameObject);
         return;
      }

      Debug.Log("AudioManager: Starting sound initialization...");
      Debug.Log($"AudioManager: Found {sounds.Length} sounds to load");

      foreach (Sound s in sounds)
      {
         s.source = gameObject.AddComponent<AudioSource>();
         s.source.clip = s.clip;
         s.source.volume = s.volume;
         s.source.pitch = s.pitch;
         s.source.loop = s.loop;
         s.source.spatialBlend = 0f;

         // Assign appropriate mixer group based on sound type
         AssignMixerGroup(s);
         
         // Log each sound that's loaded
         if (s.clip != null)
         {
            Debug.Log($"AudioManager: Loaded sound '{s.name}' - Type: {s.type}, Clip: {s.clip.name}, Length: {s.clip.length}s");
         }
         else
         {
            Debug.LogWarning($"AudioManager: Sound '{s.name}' has no audio clip assigned!");
         }
      }
      
      Debug.Log("AudioManager: Sound initialization completed successfully");
   }

   private void Start()
   {
      Debug.Log("AudioManager: Initializing volume settings and scheduler reference");
      InitializeVolume();
      
      // Get reference to AudioScheduler using non-deprecated method
      scheduler = FindFirstObjectByType<AudioScheduler>();
      if (scheduler == null)
      {
         Debug.LogWarning("AudioManager: No AudioScheduler found in scene");
      }
      else
      {
         Debug.Log("AudioManager: AudioScheduler reference obtained successfully");
      }
   }

   private void AssignMixerGroup(Sound sound)
   {
      switch (sound.type)
      {
         case SoundType.Player:
         case SoundType.Enemy:
         case SoundType.Weapon:
         case SoundType.Item:
            sound.source.outputAudioMixerGroup = sfxMixerGroup;
            break;
         case SoundType.UI:
            sound.source.outputAudioMixerGroup = uiMixerGroup;
            break;
         default:
            sound.source.outputAudioMixerGroup = sfxMixerGroup;
            break;
      }
   }

   private void InitializeVolume()
   {
      // Initialize single volume slider if assigned
      if (volumeSlider != null)
      {
         if (!PlayerPrefs.HasKey("MasterVolume"))
         {
            PlayerPrefs.SetFloat("MasterVolume", 1f);
            Debug.Log("AudioManager: No saved volume found, setting default volume to 1");
         }
         
         float savedVolume = PlayerPrefs.GetFloat("MasterVolume");
         volumeSlider.value = savedVolume;
         ChangeVolume(); // Apply the saved volume
         
         Debug.Log($"AudioManager: Volume slider initialized with value: {savedVolume}");
      }
      else
      {
         // Set default volume even without slider
         if (!PlayerPrefs.HasKey("MasterVolume"))
         {
            PlayerPrefs.SetFloat("MasterVolume", 1f);
         }
         
         float savedVolume = PlayerPrefs.GetFloat("MasterVolume");
         if (audioMixer != null)
         {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(savedVolume) * 20);
         }
         
         Debug.Log($"AudioManager: Volume initialized without slider: {savedVolume}");
      }
   }

   public void Play(string name)
   {
      if (Instance == null)
      {
         Debug.LogError("AudioManager instance is null!");
         return;
      }

      Sound s = Array.Find(sounds, sound => sound.name == name);
      
      if (s == null)
      {
         Debug.LogWarning("Sound: " + name + " not found!");
         return;
      }
      
      if (s.source == null)
      {
         Debug.LogError($"AudioManager: Sound source for '{name}' is null!");
         return;
      }

      // Check with AudioScheduler if we can play this sound
      if (scheduler != null && !scheduler.CanPlaySound(name))
      {
         Debug.Log($"AudioManager: Sound '{name}' is on cooldown, skipping playback");
         return;
      }
      
      Debug.Log($"AudioManager: Playing sound '{name}' (Loop: {s.loop})");
      s.source.Play();

      // Record play time for cooldown management
      if (scheduler != null)
      {
         scheduler.RecordPlayTime(name);
      }
   }

   public void PlayOneShot(string name)
   {
      if (Instance == null)
      {
         Debug.LogError("AudioManager instance is null!");
         return;
      }

      Sound s = Array.Find(sounds, sound => sound.name == name);
      
      if (s == null)
      {
         Debug.LogWarning("Sound: " + name + " not found!");
         return;
      }
      
      if (s.clip == null)
      {
         Debug.LogError($"AudioManager: Audio clip for '{name}' is null!");
         return;
      }

      // Check with AudioScheduler if we can play this sound
      if (scheduler != null && !scheduler.CanPlaySound(name))
      {
         Debug.Log($"AudioManager: Sound '{name}' is on cooldown, skipping one-shot playback");
         return;
      }
      
      Debug.Log($"AudioManager: Playing one-shot sound '{name}'");
      s.source.PlayOneShot(s.clip);

      // Record play time for cooldown management
      if (scheduler != null)
      {
         scheduler.RecordPlayTime(name);
      }
   }

   public void ChangeVolume()
   {
      // Only change volume if we have a slider
      if (volumeSlider != null && audioMixer != null)
      {
         float newVolume = volumeSlider.value;
         audioMixer.SetFloat("MasterVolume", Mathf.Log10(newVolume) * 20);
         Debug.Log($"AudioManager: Volume changed to {newVolume}");
         SaveVolume();
      }
   }

   public void SetVolume(float volume)
   {
      // Programmatic volume setting without UI slider
      if (audioMixer != null)
      {
         audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
         PlayerPrefs.SetFloat("MasterVolume", volume);
         Debug.Log($"AudioManager: Volume set to {volume}");
      }
   }

   private void SaveVolume()
   {
      if (volumeSlider != null)
      {
         PlayerPrefs.SetFloat("MasterVolume", volumeSlider.value);
      }
   }

   public void LoadVolume()
   {
      if (volumeSlider != null)
      {
         volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
      }
   }

   public void Stop(string name)
   {
      if (Instance == null)
      {
         Debug.LogError("AudioManager instance is null!");
         return;
      }

      Sound s = Array.Find(sounds, sound => sound.name == name);
      
      if (s == null)
      {
         Debug.LogWarning("Sound: " + name + " not found!");
         return;
      }
      
      Debug.Log($"AudioManager: Stopping sound '{name}'");
      s.source.Stop();
   }

   public void Pause(string name)
   {
      Sound s = Array.Find(sounds, sound => sound.name == name);
      if (s != null && s.source != null)
      {
         s.source.Pause();
         Debug.Log($"AudioManager: Paused sound '{name}'");
      }
   }

   public void UnPause(string name)
   {
      Sound s = Array.Find(sounds, sound => sound.name == name);
      if (s != null && s.source != null)
      {
         s.source.UnPause();
         Debug.Log($"AudioManager: Unpaused sound '{name}'");
      }
   }

   public bool IsSoundPlaying(string name)
   {
      Sound s = Array.Find(sounds, sound => sound.name == name);
      return s != null && s.source != null && s.source.isPlaying;
   }

   public System.Collections.IEnumerator FadeOut(string soundName, float fadeTime)
   {
      Sound s = Array.Find(sounds, sound => sound.name == soundName);
      if (s == null || s.source == null) yield break;

      float startVolume = s.source.volume;
      
      while (s.source.volume > 0)
      {
         s.source.volume -= startVolume * Time.deltaTime / fadeTime;
         yield return null;
      }
      
      s.source.Stop();
      s.source.volume = startVolume;
      Debug.Log($"AudioManager: Faded out sound '{soundName}' over {fadeTime} seconds");
   }

   public System.Collections.IEnumerator FadeIn(string soundName, float fadeTime, float targetVolume = 1f)
   {
      Sound s = Array.Find(sounds, sound => sound.name == soundName);
      if (s == null || s.source == null) yield break;

      s.source.volume = 0f;
      s.source.Play();
      
      while (s.source.volume < targetVolume)
      {
         s.source.volume += targetVolume * Time.deltaTime / fadeTime;
         yield return null;
      }
      
      s.source.volume = targetVolume;
      Debug.Log($"AudioManager: Faded in sound '{soundName}' over {fadeTime} seconds");
   }

   public void DebugSounds()
   {
      Debug.Log("=== AudioManager Sound Debug Info ===");
      Debug.Log($"Total sounds: {sounds.Length}");
      
      foreach (Sound s in sounds)
      {
         string status = s.source != null && s.source.isPlaying ? "PLAYING" : "stopped";
         Debug.Log($"- '{s.name}': Type={s.type}, Clip={(s.clip != null ? s.clip.name : "NULL")}, Status={status}");
      }
      Debug.Log("=====================================");
   }
}