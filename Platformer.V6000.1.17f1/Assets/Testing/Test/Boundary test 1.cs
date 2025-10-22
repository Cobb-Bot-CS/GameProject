using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.TestTools;

public class AudioBoundaryTests
{
    private SoundManager sm;
    private AudioMixer mixer;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        var go = new GameObject("AudioBoundaryHarness");
        sm = go.AddComponent<SoundManager>();
        mixer = Resources.Load<AudioMixer>("MasterMixer");
        Assert.IsNotNull(sm);
        Assert.IsNotNull(mixer);
        yield return null;
    }

    [UnityTest]
    public IEnumerator VolumeBoundaries_NoOutputAtZero_NoClipAtOneWithLimiter()
    {
        // Min boundary: silent
        sm.Play("TestToneHot", Vector3.zero, 0f, 0f);
        yield return new WaitForSeconds(0.2f);
        float peakDbSilent = GetPeakDb("MasterPeak");
        Assert.Less(peakDbSilent, -80f, "Expected near-silence at volume=0");

        // Max boundary with limiter ON
        SetLimiter(true);
        sm.Play("TestToneHot", Vector3.zero, 1f, 0f);
        yield return new WaitForSeconds(0.2f);
        float peakDbLimited = GetPeakDb("MasterPeak");
        Assert.LessOrEqual(peakDbLimited, 0f, "Limiter should prevent clipping at volume=1");

        // Max boundary with limiter OFF (expect near-clip or clip for test validation)
        SetLimiter(false);
        sm.Play("TestToneHot", Vector3.zero, 1f, 0f);
        yield return new WaitForSeconds(0.2f);
        float peakDbUnlimited = GetPeakDb("MasterPeak");
        Assert.Greater(peakDbUnlimited, -0.5f, "Without limiter, hot tone should approach clip");

        yield return null;
    }

    private float GetPeakDb(string exposedParam)
    {
        float val;
        mixer.GetFloat(exposedParam, out val);
        return val;
    }

    private void SetLimiter(bool enabled)
    {
        // Example: toggle a mixer snapshot or an exposed Dry/Wet for a limiter insert
        mixer.TransitionToSnapshots(
            new AudioMixerSnapshot[] { mixer.FindSnapshot(enabled ? "LimitingOn" : "LimitingOff") },
            new float[] { 1f },
            0.05f
        );
    }
}
