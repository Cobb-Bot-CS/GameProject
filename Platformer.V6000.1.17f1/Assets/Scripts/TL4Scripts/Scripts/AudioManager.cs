
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Audio;

/*
 * filename: AudioManager.cs
 * Developer: Urvashi Gupta
 * Purpose: Manages audio playback, volume settings, and sound effects throughout the game
 */

/*
 * Summary: Singleton class that handles all audio operations including playback, volume control, and persistence
 *
 * Member Variables:
 * volumeSlider - UI slider for controlling master volume
 * Instance - singleton instance of AudioManager
 * sounds - array of Sound objects containing audio clips and settings
 * musicMixerGroup - AudioMixerGroup for music and ambient sounds
 * sfxMixerGroup - AudioMixerGroup for sound effects
 * uiMixerGroup - AudioMixerGroup for UI sounds
 */
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

      //DontDestroyOnLoad(gameObject);

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
      }
   }



   /*
    * Summary: Assigns the correct AudioMixerGroup based on the sound type
    *
    * Parameters:
    * sound - the Sound object to assign mixer group to
    */
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



   /*
    * Summary: Initializes volume settings from player preferences
    */
   private void Start()
   {
      if (!PlayerPrefs.HasKey("musicVolume"))
      {
         PlayerPrefs.SetFloat("musicVolume", 1);
      }
      else
      {
         Load();
      }
   }



   /*
    * Summary: Plays a sound by name with looping capability
    *
    * Parameters:
    * name - the name of the sound to play
    */
   public void Play(string name)
   {
      Sound s = Array.Find(sounds, sound => sound.name == name);
      
      if (s == null)
      {
         Debug.LogWarning("Sound: " + name + " not found!");
         return;
      }
      
      s.source.Play();
   }



   /*
    * Summary: Plays a sound once without looping
    *
    * Parameters:
    * name - the name of the sound to play once
    */
   public void PlayOneShot(string name)
   {
      Sound s = Array.Find(sounds, sound => sound.name == name);
      
      if (s == null)
      {
         Debug.LogWarning("Sound: " + name + " not found!");
         return;
      }
      
      s.source.PlayOneShot(s.clip);
   }



   /*
    * Summary: Changes the master volume based on slider value and saves preference
    */
   public void ChangeVolume()
   {
      AudioListener.volume = volumeSlider.value;
      Save();
   }



   /*
    * Summary: Loads volume setting from player preferences
    */
  
   public void Load()
   {
      volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
   }
   


   /*
    * Summary: Saves current volume setting to player preferences
    */
   
   
   public void Save()
   {
      PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
   }
}

