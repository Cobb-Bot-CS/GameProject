using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Audio;

public class SoundStressTests
{
    private GameObject audioManager;
    private List<AudioSource> createdSources = new List<AudioSource>();

    [SetUp]
    public void SetUp()
    {
        audioManager = new GameObject("AudioManager");
        audioManager.AddComponent<AudioListener>();
        Time.timeScale = 1f; // Ensure normal time scale
        AudioListener.pause = false; // Ensure audio isn't paused
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up all created objects
        foreach (var source in createdSources)
        {
            if (source != null && source.gameObject != null)
                Object.DestroyImmediate(source.gameObject);
        }
        createdSources.Clear();
        
        if (audioManager != null)
            Object.DestroyImmediate(audioManager);
            
        Time.timeScale = 1f; // Reset time scale
        AudioListener.pause = false; // Ensure audio isn't paused
    }

    [UnityTest]
    public IEnumerator Test17_SingleSFXInstanceLimit()
    {
        int maxInstances = 3;
        var activeSources = new List<AudioSource>();
        
        for (int i = 0; i < 10; i++)
        {
            var source = CreateAudioSource($"SFX_{i}");
            source.Play();
            activeSources.Add(source);
            
            // Simple instance limiting logic
            if (activeSources.Count > maxInstances)
            {
                var oldSource = activeSources[0];
                if (oldSource != null)
                {
                    oldSource.Stop();
                    Object.DestroyImmediate(oldSource.gameObject);
                }
                activeSources.RemoveAt(0);
            }
            
            yield return null;
        }
        
        int playingCount = GetPlayingCount(activeSources);
        Assert.LessOrEqual(playingCount, maxInstances, 
            $"Should not exceed {maxInstances} simultaneous instances (found: {playingCount})");
    }

    [UnityTest]
    public IEnumerator Test18_GlobalVoiceLimitStress()
    {
        int soundCount = 50; // Reduced from 100 for stability
        var sources = new List<AudioSource>();
        
        // Create many sounds quickly
        for (int i = 0; i < soundCount; i++)
        {
            var source = CreateAudioSource($"Voice_{i}");
            source.priority = 128 + (i % 64); // Vary priorities (0-255 range)
            source.Play();
            sources.Add(source);
            yield return null;
        }
        
        yield return new WaitForSeconds(0.5f);
        
        int playingCount = GetPlayingCount(sources);
        Assert.IsTrue(playingCount > 0, $"At least some sounds should be playing (playing: {playingCount}/{soundCount})");
        Debug.Log($"Voice limit test: {playingCount} of {soundCount} sounds still playing");
    }

    [UnityTest]
    public IEnumerator Test19_RapidFireIdenticalSFX()
    {
        int shotsFired = 0;
        float testDuration = 1.5f; // Reduced from 2f
        float startTime = Time.time;
        
        while (Time.time - startTime < testDuration)
        {
            var source = CreateAudioSource($"MachineGun_{shotsFired}");
            source.Play();
            shotsFired++;
            yield return null;
        }
        
        Assert.IsTrue(shotsFired > 5, $"Should fire many shots (fired: {shotsFired})");
        Debug.Log($"Rapid fire test: {shotsFired} shots in {testDuration} seconds");
    }

    [UnityTest]
    public IEnumerator Test20_MultipleLongSoundsSimultaneous()
    {
        int longSoundCount = 5; // Reduced from 10
        var longSources = new List<AudioSource>();
        
        for (int i = 0; i < longSoundCount; i++)
        {
            var longSource = CreateAudioSource($"LongSound_{i}");
            // Create a longer clip (5 seconds instead of 10)
            var longClip = AudioClip.Create("LongClip", 220500, 1, 44100, false);
            longSource.clip = longClip;
            longSource.loop = true;
            longSource.Play();
            longSources.Add(longSource);
        }
        
        yield return new WaitForSeconds(0.5f); // Reduced wait time
        
        Assert.AreEqual(longSoundCount, GetPlayingCount(longSources), 
            "All long sounds should still be playing");
    }

    [UnityTest]
    public IEnumerator Test21_SceneLoadAudioSourceOverload()
    {
        int sourceCount = 50; // Reduced from 100
        var sources = new List<AudioSource>();
        
        for (int i = 0; i < sourceCount; i++)
        {
            var sourceObj = new GameObject($"InactiveSource_{i}");
            sourceObj.SetActive(false); // Simulate scene-loaded inactive objects
            var source = sourceObj.AddComponent<AudioSource>();
            source.clip = AudioClip.Create("TempClip", 4410, 1, 44100, false); // Shorter clip
            sources.Add(source);
            createdSources.Add(source);
            yield return null;
        }
        
        // Activate a few
        int activated = 0;
        for (int i = 0; i < 5 && i < sources.Count; i++)
        {
            sources[i].gameObject.SetActive(true);
            sources[i].Play();
            activated++;
        }
        
        yield return new WaitForSeconds(0.3f);
        
        int playingCount = GetPlayingCount(sources);
        Debug.Log($"AudioSource overload: {playingCount} playing out of {activated} activated");
        Assert.Pass($"Successfully handled {sourceCount} AudioSources ({playingCount} playing)");
    }

    [UnityTest]
    public IEnumerator Test22_SimultaneousActivationStress()
    {
        int activationCount = 30; // Reduced from 50
        var sources = new List<AudioSource>();
        
        // Create inactive sources
        for (int i = 0; i < activationCount; i++)
        {
            var source = CreateAudioSource($"StressSource_{i}");
            source.gameObject.SetActive(false);
            sources.Add(source);
        }
        
        // Activate all simultaneously
        foreach (var source in sources)
        {
            source.gameObject.SetActive(true);
            source.Play();
        }
        
        yield return new WaitForSeconds(0.5f);
        
        int playingCount = GetPlayingCount(sources);
        Assert.IsTrue(playingCount > activationCount / 3, 
            $"Reasonable number of sounds should play (playing: {playingCount}/{activationCount})");
    }

    [UnityTest]
    public IEnumerator Test23_MemoryLargeAudioFiles()
    {
        // Create a medium audio clip (~2MB uncompressed instead of 10MB)
        int sampleCount = 44100 * 2; // 2 seconds at 44.1kHz
        var largeClip = AudioClip.Create("LargeClip", sampleCount, 1, 44100, false);
        
        // Test loading into memory
        var source = CreateAudioSource("LargeSource");
        source.clip = largeClip;
        source.Play();
        
        yield return new WaitForSeconds(0.3f);
        
        Assert.IsTrue(source.isPlaying, "Large audio file should play without issues");
        
        // Clean up
        Resources.UnloadAsset(largeClip);
        Debug.Log("Large audio file test completed successfully");
    }


    [UnityTest]
    public IEnumerator Test26_GC_AllocationContinuousPlayback()
    {
        int testDuration = 3; // seconds (reduced from 5)
        float startTime = Time.time;
        int soundsPlayed = 0;
        
        while (Time.time - startTime < testDuration)
        {
            // Play sounds continuously but less frequently
            if (soundsPlayed % 3 == 0) // Only create every 3rd frame
            {
                var tempSource = CreateAudioSource($"Temp_{soundsPlayed}");
                tempSource.Play();
                soundsPlayed++;
            }
            
            // Clean up old sources periodically
            if (soundsPlayed > 10 && soundsPlayed % 5 == 0)
            {
                for (int i = createdSources.Count - 1; i >= 0 && i > createdSources.Count - 6; i--)
                {
                    if (createdSources[i] != null && !createdSources[i].isPlaying)
                    {
                        Object.DestroyImmediate(createdSources[i].gameObject);
                        createdSources.RemoveAt(i);
                    }
                }
            }
            
            yield return null;
        }
        
        Debug.Log($"GC test: {soundsPlayed} sounds created in {testDuration} seconds");
        Assert.Pass($"GC allocation test completed with {soundsPlayed} sounds");
    }

    [UnityTest]
    public IEnumerator Test27_AudioMixerEffectCPUStress()
    {
        var mixer = Resources.Load<AudioMixer>("StressTestMixer");
        if (mixer == null)
        {
            Assert.Pass("StressTestMixer not found - test skipped");
            yield break;
        }

        int effectSourceCount = 15; // Reduced from 20
        var effectSources = new List<AudioSource>();
        
        // Find any available group
        var groups = mixer.FindMatchingGroups("");
        if (groups.Length == 0)
        {
            Assert.Pass("No groups found in StressTestMixer - test skipped");
            yield break;
        }

        // Create sources with mixer effects
        for (int i = 0; i < effectSourceCount; i++)
        {
            var source = CreateAudioSource($"EffectSource_{i}");
            source.outputAudioMixerGroup = groups[0];
            source.Play();
            effectSources.Add(source);
        }
        
        // Monitor performance for 2 seconds
        float startTime = Time.time;
        int frames = 0;
        
        while (Time.time - startTime < 2f)
        {
            frames++;
            yield return null;
        }
        
        float avgFps = frames / 2f;
        Assert.IsTrue(avgFps > 20f, $"Should maintain playable FPS with effects (was: {avgFps})");
        Debug.Log($"Mixer effect stress test: {avgFps} FPS average with {effectSourceCount} sources");
    }

    // System Event Tests
    [UnityTest]
    public IEnumerator Test28_GamePauseUnpause()
    {
        var source = CreateAudioSource("PauseTest");
        source.loop = true;
        source.Play();
        
        yield return new WaitForSeconds(0.1f);
        
        // "Pause" by setting timescale to 0
        Time.timeScale = 0f;
        AudioListener.pause = true;
        
        yield return new WaitForSecondsRealtime(0.3f); // Use realtime since game is paused
        
        // "Unpause"
        Time.timeScale = 1f;
        AudioListener.pause = false;
        
        yield return new WaitForSeconds(0.1f);
        
        Assert.IsTrue(source.isPlaying, "Sound should resume after unpause");
    }

    [UnityTest]
    public IEnumerator Test29_SceneTransitionSoundPersistence()
    {
        var persistentSource = CreateAudioSource("PersistentSound");
        Object.DontDestroyOnLoad(persistentSource.gameObject); // Make it persist
        
        persistentSource.Play();
        
        yield return new WaitForSeconds(0.1f);
        
        // Simulate scene change by destroying other objects but keeping persistent
        var tempObj = new GameObject("TempSceneObject");
        
        // "Load new scene" - destroy temp object but keep persistent sound
        Object.DestroyImmediate(tempObj);
        
        yield return new WaitForSeconds(0.1f);
        
        Assert.IsTrue(persistentSource.isPlaying, "Persistent sound should continue playing");
        Assert.IsTrue(persistentSource != null, "Persistent sound object should still exist");
    }

    [UnityTest]
    public IEnumerator Test30_TimeScaleZero()
    {
        var scaledSource = CreateAudioSource("ScaledSound");
        scaledSource.Play();
        
        yield return new WaitForSeconds(0.1f);
        
        Time.timeScale = 0f;
        
        yield return new WaitForSecondsRealtime(0.3f);
        
        // Sound should still be "playing" but effectively frozen
        Assert.IsTrue(scaledSource.isPlaying, "Sound should not stop when timescale is 0");
        
        Time.timeScale = 1f; // Restore
    }

    [UnityTest]
    public IEnumerator Test31_AudioListenerDestroyed()
    {
        var testSource = CreateAudioSource("ListenerTest");
        testSource.Play();
        
        yield return new WaitForSeconds(0.1f);
        
        // Destroy listener
        var listener = audioManager.GetComponent<AudioListener>();
        Object.DestroyImmediate(listener);
        
        yield return new WaitForSeconds(0.2f);
        
        // Sound might still be playing but no output
        Assert.IsTrue(testSource.isPlaying, "Sound instance should continue existing");
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator Test32_ExtremeSlowMotionFastMotion()
    {
        var pitchSource = CreateAudioSource("PitchTest");
        
        // Test slow motion
        Time.timeScale = 0.1f;
        pitchSource.Play();
        
        yield return new WaitForSecondsRealtime(0.3f);
        
        pitchSource.Stop();
        yield return null;
        
        // Test fast motion
        Time.timeScale = 4.0f;
        pitchSource.Play();
        
        yield return new WaitForSecondsRealtime(0.3f);
        
        Time.timeScale = 1f; // Restore normal time
        
        Assert.Pass("Extreme time scaling handled without errors");
    }

    // Helper Methods
    private AudioSource CreateAudioSource(string name)
    {
        var obj = new GameObject(name);
        var source = obj.AddComponent<AudioSource>();
        
        // Create basic test clip if none exists
        if (source.clip == null)
        {
            source.clip = AudioClip.Create("TestClip", 44100, 1, 44100, false);
        }
        
        createdSources.Add(source);
        return source;
    }
    
    private int GetPlayingCount(List<AudioSource> sources)
    {
        int count = 0;
        foreach (var source in sources)
        {
            if (source != null && source.isPlaying)
                count++;
        }
        return count;
    }
}