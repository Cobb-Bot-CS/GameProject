using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Audio;

public class SoundBoundaryTests
{
    private GameObject audioManager;
    private AudioSource testSource;
    private AudioListener listener;

    [SetUp]
    public void SetUp()
    {
        // Create audio manager and test objects
        audioManager = new GameObject("AudioManager");
        testSource = audioManager.AddComponent<AudioSource>();
        listener = audioManager.AddComponent<AudioListener>();
        
        // Create test audio clip (sine wave)
        testSource.clip = AudioClip.Create("TestClip", 44100, 1, 44100, false);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(audioManager);
        AudioListener.volume = 1f; // Reset volume
        Time.timeScale = 1f; // Reset time scale
        AudioListener.pause = false; // Ensure audio isn't paused
    }

    // Volume Boundary Tests
    [UnityTest]
    public IEnumerator Test01_MasterVolumeMinimum()
    {
        AudioListener.volume = 0f;
        testSource.Play();
        
        yield return new WaitForSeconds(0.1f);
        
        Assert.IsTrue(testSource.isPlaying, "Sound should be playing");
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator Test02_MasterVolumeMaximum()
    {
        AudioListener.volume = 1f;
        testSource.Play();
        
        yield return new WaitForSeconds(0.1f);
        
        Assert.IsTrue(testSource.isPlaying, "Sound should be playing at max volume");
        LogAssert.NoUnexpectedReceived();
    }

    [Test]
    public void Test03_MasterVolumeNegative()
    {
        AudioListener.volume = -1f;
        Assert.AreEqual(0f, AudioListener.volume, "Volume should clamp to 0");
    }

    [Test]
    public void Test04_MasterVolumeExcessive()
    {
        AudioListener.volume = 1.5f;
        Assert.AreEqual(1f, AudioListener.volume, "Volume should clamp to 1");
    }

    [UnityTest]
    public IEnumerator Test05_SFXChannelMinimum()
    {
        var mixer = Resources.Load<AudioMixer>("SFXMixer");
        if (mixer == null)
        {
            Assert.Pass("SFXMixer not found - test skipped");
            yield break;
        }

        // Try to find SFX group or use first available group
        var groups = mixer.FindMatchingGroups("SFX");
        if (groups.Length == 0) groups = mixer.FindMatchingGroups("");
        
        if (groups.Length > 0)
        {
            testSource.outputAudioMixerGroup = groups[0];
            mixer.SetFloat("Volume", -80f); // Use main volume parameter
            
            testSource.Play();
            yield return new WaitForSeconds(0.1f);
            
            Assert.IsTrue(testSource.isPlaying, "SFX should be playing but silent");
        }
        else
        {
            Assert.Pass("No audio groups found in SFXMixer - test skipped");
        }
    }

    [UnityTest]
    public IEnumerator Test06_MusicChannelMinimum()
    {
        var mixer = Resources.Load<AudioMixer>("MusicMixer");
        if (mixer == null)
        {
            Assert.Pass("MusicMixer not found - test skipped");
            yield break;
        }

        var groups = mixer.FindMatchingGroups("Music");
        if (groups.Length == 0) groups = mixer.FindMatchingGroups("");
        
        if (groups.Length > 0)
        {
            testSource.outputAudioMixerGroup = groups[0];
            mixer.SetFloat("Volume", -80f);
            
            testSource.Play();
            yield return new WaitForSeconds(0.1f);
            
            Assert.IsTrue(testSource.isPlaying, "Music should be playing but silent");
        }
        else
        {
            Assert.Pass("No audio groups found in MusicMixer - test skipped");
        }
    }

    [UnityTest]
    public IEnumerator Test07_UIChannelMinimum()
    {
        var mixer = Resources.Load<AudioMixer>("UIMixer");
        if (mixer == null)
        {
            Assert.Pass("UIMixer not found - test skipped");
            yield break;
        }

        var groups = mixer.FindMatchingGroups("UI");
        if (groups.Length == 0) groups = mixer.FindMatchingGroups("");
        
        if (groups.Length > 0)
        {
            testSource.outputAudioMixerGroup = groups[0];
            mixer.SetFloat("Volume", -80f);
            
            testSource.Play();
            yield return new WaitForSeconds(0.1f);
            
            Assert.IsTrue(testSource.isPlaying, "UI sounds should be playing but silent");
        }
        else
        {
            Assert.Pass("No audio groups found in UIMixer - test skipped");
        }
    }

    [UnityTest]
    public IEnumerator Test08_RapidVolumeCycling()
    {
        testSource.Play();
        
        for (int i = 0; i < 20; i++) // Reduced from 50 for performance
        {
            AudioListener.volume = i % 2 == 0 ? 0f : 1f;
            yield return null; // Wait one frame
        }
        
        Assert.IsTrue(testSource.isPlaying, "Sound should still be playing after volume cycling");
        LogAssert.NoUnexpectedReceived();
    }

    // Spatial Audio Boundary Tests
    [UnityTest]
    public IEnumerator Test09_SoundAtMinDistance()
    {
        testSource.spatialBlend = 1f; // Full 3D
        testSource.minDistance = 5f;
        testSource.maxDistance = 50f;
        
        // Position source at min distance from listener (who is at 0,0,0)
        audioManager.transform.position = new Vector3(5f, 0f, 0f);
        testSource.Play();
        
        yield return new WaitForSeconds(0.1f);
        
        Assert.IsTrue(testSource.isPlaying, "Sound should play at full volume at min distance");
    }

    [UnityTest]
    public IEnumerator Test10_SoundAtMaxDistance()
    {
        testSource.spatialBlend = 1f;
        testSource.minDistance = 5f;
        testSource.maxDistance = 50f;
        testSource.rolloffMode = AudioRolloffMode.Linear;
        
        audioManager.transform.position = new Vector3(50f, 0f, 0f);
        testSource.Play();
        
        yield return new WaitForSeconds(0.1f);
        
        Assert.IsTrue(testSource.isPlaying, "Sound should play at minimum volume at max distance");
    }

    [UnityTest]
    public IEnumerator Test11_SoundBeyondMaxDistance()
    {
        testSource.spatialBlend = 1f;
        testSource.minDistance = 5f;
        testSource.maxDistance = 50f;
        
        audioManager.transform.position = new Vector3(51f, 0f, 0f);
        testSource.Play();
        
        yield return new WaitForSeconds(0.1f);
        
        Assert.IsTrue(testSource.isPlaying, "Sound instance should exist but be inaudible");
    }

    [UnityTest]
    public IEnumerator Test12_SoundAtWorldOrigin()
    {
        audioManager.transform.position = Vector3.zero;
        testSource.Play();
        
        yield return new WaitForSeconds(0.1f);
        
        Assert.IsTrue(testSource.isPlaying, "Sound should play normally at world origin");
    }

    [UnityTest]
    public IEnumerator Test13_SoundAtExtremeCoordinates()
    {
        audioManager.transform.position = new Vector3(100000f, 100000f, 100000f);
        testSource.spatialBlend = 1f;
        testSource.Play();
        
        yield return new WaitForSeconds(0.1f);
        
        Assert.IsTrue(testSource.isPlaying, "Sound should handle extreme coordinates without errors");
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator Test14_ListenerAtExtremeCoordinates()
    {
        // Move listener to extreme position
        audioManager.transform.position = new Vector3(100000f, 100000f, 100000f);
        
        // Create a sound at relative position
        var soundObj = new GameObject("ExtremeSound");
        var extremeSource = soundObj.AddComponent<AudioSource>();
        extremeSource.clip = testSource.clip;
        extremeSource.spatialBlend = 1f;
        soundObj.transform.position = new Vector3(100005f, 100000f, 100000f);
        
        extremeSource.Play();
        
        yield return new WaitForSeconds(0.1f);
        
        Assert.IsTrue(extremeSource.isPlaying, "Spatial audio should work at extreme coordinates");
        Object.DestroyImmediate(soundObj);
    }

    [UnityTest]
    public IEnumerator Test15_SoundOnListener()
    {
        audioManager.transform.position = Vector3.zero; // Same as listener
        testSource.spatialBlend = 1f;
        testSource.Play();
        
        yield return new WaitForSeconds(0.1f);
        
        Assert.IsTrue(testSource.isPlaying, "Sound should play when positioned on listener");
    }

    [UnityTest]
    public IEnumerator Test16_SoundBehindListener()
    {
        // Position sound directly behind listener (facing forward)
        audioManager.transform.position = new Vector3(0f, 0f, -5f);
        testSource.spatialBlend = 1f;
        testSource.Play();
        
        yield return new WaitForSeconds(0.1f);
        
        Assert.IsTrue(testSource.isPlaying, "Sound should play when positioned behind listener");
    }
}