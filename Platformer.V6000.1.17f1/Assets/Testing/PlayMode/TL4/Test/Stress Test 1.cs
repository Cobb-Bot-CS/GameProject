using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class StressTest_VoiceLimit
{
    private AudioClip testClip;
    private GameObject listener;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        testClip = Resources.Load<AudioClip>("SFX/MusicSFX/DinoArea");
        Assert.IsNotNull(testClip, "Test clip not found in Resources/SFX/MusicSFX");
        
        // Create Audio Listener - CRITICAL for audio to work!
        listener = new GameObject("TestAudioListener");
        listener.AddComponent<AudioListener>();
        
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        if (listener != null)
            Object.Destroy(listener);
        yield return null;
    }

    [UnityTest]
    public IEnumerator FindVoiceLimit()
    {
        Debug.Log("=== STRESS TEST: Finding Voice Limit ===");
        
        int failurePoint = -1;
        int maxVoices = 128; // Test up to 128 voices
        
        for (int voiceCount = 4; voiceCount <= maxVoices; voiceCount += 4)
        {
            Debug.Log($"Testing {voiceCount} simultaneous voices...");
            
            var sources = new AudioSource[voiceCount];
            
            // Create all audio sources
            for (int i = 0; i < voiceCount; i++)
            {
                var go = new GameObject($"Voice_{i}");
                var src = go.AddComponent<AudioSource>();
                src.clip = testClip;
                src.Play();
                sources[i] = src;
            }

            // Wait for audio to start
            yield return new WaitForSeconds(0.2f);

            // Count how many are actually playing
            int reallyPlaying = 0;
            foreach (var src in sources)
            {
                if (src != null && src.isPlaying && src.time > 0f)
                    reallyPlaying++;
            }

            Debug.Log($"Voices requested: {voiceCount}, Actually playing: {reallyPlaying}");

            // Check if we hit the limit
            if (reallyPlaying < voiceCount)
            {
                failurePoint = voiceCount;
                Debug.LogWarning($"⚠️ LIMIT REACHED: Virtualization started at {voiceCount} voices");
                Debug.LogWarning($"Only {reallyPlaying}/{voiceCount} voices actually playing");
                
                // Cleanup
                foreach (var src in sources)
                    if (src != null) Object.Destroy(src.gameObject);
                
                break;
            }

            // Cleanup
            foreach (var src in sources)
                if (src != null) Object.Destroy(src.gameObject);

            yield return new WaitForSeconds(0.1f);
        }

        // Report results
        if (failurePoint == -1)
        {
            Assert.Fail($"Stress test did not find voice limit up to {maxVoices} voices. Test may need adjustment.");
        }
        else
        {
            Assert.Fail($"VOICE LIMIT FOUND: System failed at {failurePoint} simultaneous voices. " +
                       $"This is the expected behavior - reporting maximum capacity.");
        }
    }
}