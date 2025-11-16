using UnityEngine;

/*
 * filename: Sound.cs
 * Developer: Urvashi Gupta
 * Purpose: Defines the Sound data structure for audio management system
 */

/*
 * Summary: Serializable class representing an audio sound with configurable properties
 *
 * Member Variables:
 * name - unique identifier for the sound
 * type - categorization of the sound (SFX, Music, UI, etc.)
 * clip - the AudioClip asset to be played
 * volume - audio volume level between 0 and 1
 * pitch - playback speed between 0.1 and 3
 * loop - whether the sound should loop continuously
 * source - AudioSource component reference (hidden in inspector)
 */
[System.Serializable]
public class Sound
{
   public string name;
   public SoundType type;
   public AudioClip clip;

   [Range(0f, 1f)]
   public float volume = 1f;
   
   [Range(.1f, 3f)]
   public float pitch = 1f;

   public bool loop;
   
   [HideInInspector]
   public AudioSource source;
}