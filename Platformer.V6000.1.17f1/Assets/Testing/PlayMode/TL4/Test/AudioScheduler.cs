using UnityEngine;
using System.Collections.Generic;

/*
 * filename: AudioScheduler.cs
 * Developer: Urvashi Gupta
 * Purpose: Schedules audio playback to prevent conflicts and manage cooldowns
 */
public class AudioScheduler : MonoBehaviour
{
   private Dictionary<string, float> lastPlayTimes = new Dictionary<string, float>();
   private Dictionary<string, float> soundCooldowns = new Dictionary<string, float>();

   private AudioManager audioManager;

   private void Start()
   {
      // Define cooldown periods for problematic sounds
      soundCooldowns = new Dictionary<string, float>()
      {
         {"PlayerDeath", 2.0f},
         {"EnemyAttack", 0.3f},
         {"PlayerHurt", 0.5f},
         {"Jump", 0.2f},
         {"SwordAttack", 0.4f}
      };

      lastPlayTimes = new Dictionary<string, float>();
      
      // Get AudioManager reference using non-deprecated method
      audioManager = FindFirstObjectByType<AudioManager>();
      if (audioManager == null)
      {
         Debug.LogError("AudioScheduler: No AudioManager found in scene!");
         return;
      }

      StartCoroutine(PlaybackMonitor());
      Debug.Log("AudioScheduler: Initialized with cooldown management");
   }

   private System.Collections.IEnumerator PlaybackMonitor()
   {
      while (true)
      {
         if (audioManager != null)
         {
            ManageSoundPlayback();
         }
         yield return new WaitForSeconds(0.05f); // Monitor 20 times per second
      }
   }

   private void ManageSoundPlayback()
   {
      // Check if PlayerDeath is playing
      if (IsSoundPlaying("PlayerDeath"))
      {
         // Stop or pause EnemyAttack sounds to reduce audio clutter during death sequence
         StopSound("EnemyAttack");
         
         // Also lower volume of other combat sounds
         FadeOutSound("SwordAttack", 0.5f);
         FadeOutSound("EnemyChase", 0.5f);
      }

      // Manage rapid fire sounds
      ManageRapidFireSounds();
   }

   private void ManageRapidFireSounds()
   {
      // Example: Limit too many hurt sounds in quick succession
      if (IsSoundPlaying("PlayerHurt") && GetSoundPlayCount("PlayerHurt") > 2)
      {
         Debug.Log("AudioScheduler: Too many PlayerHurt sounds, limiting playback");
         // Additional logic to limit excessive sound spam
      }
   }

   public bool CanPlaySound(string soundName)
   {
      // If no cooldown defined, always allow playback
      if (!soundCooldowns.ContainsKey(soundName)) 
         return true;
      
      if (lastPlayTimes.ContainsKey(soundName))
      {
         float timeSinceLastPlay = Time.time - lastPlayTimes[soundName];
         bool canPlay = timeSinceLastPlay >= soundCooldowns[soundName];
         
         if (!canPlay)
         {
            Debug.Log($"AudioScheduler: Sound '{soundName}' on cooldown. {timeSinceLastPlay:F2}s / {soundCooldowns[soundName]}s");
         }
         
         return canPlay;
      }
      
      return true;
   }

   public void RecordPlayTime(string soundName)
   {
      lastPlayTimes[soundName] = Time.time;
      Debug.Log($"AudioScheduler: Recorded play time for '{soundName}' at {Time.time}");
   }

   private bool IsSoundPlaying(string soundName)
   {
      if (audioManager == null) return false;
      return audioManager.IsSoundPlaying(soundName);
   }

   private void StopSound(string soundName)
   {
      if (audioManager == null) return;
      audioManager.Stop(soundName);
      Debug.Log($"AudioScheduler: Stopped sound '{soundName}'");
   }

   private void FadeOutSound(string soundName, float fadeTime)
   {
      if (audioManager == null) return;
      
      if (IsSoundPlaying(soundName))
      {
         StartCoroutine(audioManager.FadeOut(soundName, fadeTime));
      }
   }

   private int GetSoundPlayCount(string soundName)
   {
      // This is a simplified implementation - in a real scenario,
      // you might want more sophisticated tracking
      return IsSoundPlaying(soundName) ? 1 : 0;
   }

   public void AddCooldown(string soundName, float cooldownTime)
   {
      soundCooldowns[soundName] = cooldownTime;
      Debug.Log($"AudioScheduler: Added cooldown for '{soundName}': {cooldownTime}s");
   }

   public void RemoveCooldown(string soundName)
   {
      if (soundCooldowns.ContainsKey(soundName))
      {
         soundCooldowns.Remove(soundName);
         Debug.Log($"AudioScheduler: Removed cooldown for '{soundName}'");
      }
   }

   public float GetTimeUntilCanPlay(string soundName)
   {
      if (!soundCooldowns.ContainsKey(soundName) || !lastPlayTimes.ContainsKey(soundName))
         return 0f;
      
      float timeSinceLastPlay = Time.time - lastPlayTimes[soundName];
      float timeRemaining = soundCooldowns[soundName] - timeSinceLastPlay;
      return Mathf.Max(0f, timeRemaining);
   }
}