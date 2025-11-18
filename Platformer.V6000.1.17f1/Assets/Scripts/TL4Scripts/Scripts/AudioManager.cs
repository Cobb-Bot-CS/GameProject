using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
   [SerializeField] private Slider volumeSlider;
   public static AudioManager Instance;
   public Sound[] sounds;

   [Header("Audio Mixer Groups")]
   [SerializeField] private AudioMixerGroup musicMixerGroup;
   [SerializeField] private AudioMixerGroup sfxMixerGroup;
   [SerializeField] private AudioMixerGroup uiMixerGroup;

   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
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

   private void Start()
   {
      Debug.Log("AudioManager: Initializing volume settings");
      
      // Only initialize volume if we have a volume slider
      if (volumeSlider != null)
      {
         if (!PlayerPrefs.HasKey("musicVolume"))
         {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Debug.Log("AudioManager: No saved volume found, setting default volume to 1");
         }
         else
         {
            Load();
            Debug.Log($"AudioManager: Loaded saved volume: {PlayerPrefs.GetFloat("musicVolume")}");
         }
      }
      else
      {
         Debug.LogWarning("AudioManager: No volume slider assigned, skipping volume initialization");
      }
   }

   public void Play(string name)
   {
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
      
      Debug.Log($"AudioManager: Playing sound '{name}' (Loop: {s.loop})");
      s.source.Play();
   }

   public void PlayOneShot(string name)
   {
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
      
      Debug.Log($"AudioManager: Playing one-shot sound '{name}'");
      s.source.PlayOneShot(s.clip);
   }

   public void ChangeVolume()
   {
      // Only change volume if we have a slider
      if (volumeSlider != null)
      {
         float newVolume = volumeSlider.value;
         AudioListener.volume = newVolume;
         Debug.Log($"AudioManager: Volume changed to {newVolume}");
         Save();
      }
   }

   public void Load()
   {
      if (volumeSlider != null)
      {
         volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
      }
   }

   public void Save()
   {
      if (volumeSlider != null)
      {
         PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
      }
   }

   public void Stop(string name)
   {
      Sound s = Array.Find(sounds, sound => sound.name == name);
      
      if (s == null)
      {
         Debug.LogWarning("Sound: " + name + " not found!");
         return;
      }
      
      Debug.Log($"AudioManager: Stopping sound '{name}'");
      s.source.Stop();
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