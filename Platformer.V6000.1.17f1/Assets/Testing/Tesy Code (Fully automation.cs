using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.TestTools;

public class AudioStressTests
{
    // Configure externally or via ScriptableObject for portability
    private const int startVoices = 8;
    private const int stepVoices = 8;
    private const int maxVoices = 256;
    private const float settleSeconds = 1.5f;
    private const float spikeFrameMs = 25f; // Failure threshold
    private const int spikeConsecutiveFrames = 30; // ~0.5s at 60 FPS
    private const float clipVolumeDbThreshold = -1.0f; // close to 0 dBFS

    private AudioMixer masterMixer;
    private SoundManager soundManager;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Boot a minimal scene or create runtime-only objects
        var go = new GameObject("AudioHarness");
        soundManager = go.AddComponent<SoundManager>(); // assumes existing component
        masterMixer = Resources.Load<AudioMixer>("MasterMixer"); // ensure included in test build
        Assert.IsNotNull(soundManager, "SoundManager missing");
        Assert.IsNotNull(masterMixer, "MasterMixer missing");
        yield return null;
    }

    [UnityTest]
    public IEnumerator ConcurrentVoicesSaturation_ReturnsFailureVoiceCount()
    {
        int failureAt = -1;
        int consecutiveSpikeFrames = 0;

        // Warm-up at startVoices
        yield return PlayVoices(startVoices, settleSeconds);

        for (int voices = startVoices; voices <= maxVoices; voices += stepVoices)
        {
            yield return PlayVoices(voices, settleSeconds);

            // Check mixer peak (requires exposed parameter or metering proxy)
            float masterPeakDb = GetMixerPeakDb(masterMixer, "MasterPeak"); // expose via script or metering bus
            bool mixerClipping = masterPeakDb >= clipVolumeDbThreshold;

            // Check frame stability
            if (Time.deltaTime * 1000f >= spikeFrameMs) consecutiveSpikeFrames++;
            else consecutiveSpikeFrames = 0;

            bool frameFailure = consecutiveSpikeFrames >= spikeConsecutiveFrames;

            // Check SoundManager pool exhaustion or play failures
            bool voiceFailure = soundManager.LastPlayFailed || soundManager.ActiveSources > soundManager.MaxSources;

            if (mixerClipping || frameFailure || voiceFailure)
            {
                failureAt = voices;
                Debug.LogWarning($"Audio stress failure at voices={failureAt} | " +
                                 $"clip={mixerClipping} frameFailure={frameFailure} voiceFailure={voiceFailure}");
                break;
            }
        }

        // Report the failure point or success
        if (failureAt == -1)
        {
            Assert.Pass("No failure detected up to max voices: " + maxVoices);
        }
        else
        {
            Assert.Fail("Failure detected at concurrent voices: " + failureAt);
        }
    }

    private IEnumerator PlayVoices(int count, float seconds)
    {
        soundManager.StopAll();
        for (int i = 0; i < count; i++)
        {
            soundManager.Play("SFX_Click", Vector3.zero, 1f, 0f); // identical short clips reduce content bias
        }
        float t = 0f;
        while (t < seconds)
        {
            t += Time.deltaTime;
            yield return null;
        }
    }

    // Stub: wire this to your metering strategy (e.g., exposed float parameter updated by a metering script)
    private float GetMixerPeakDb(AudioMixer mixer, string exposedParam)
    {
        float val;
        mixer.GetFloat(exposedParam, out val);
        return val;
    }
}
