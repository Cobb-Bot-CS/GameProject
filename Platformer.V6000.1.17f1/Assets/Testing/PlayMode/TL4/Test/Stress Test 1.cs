using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class StressTest_VoiceLimit
{
    private AudioClip testClip;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        testClip = Resources.Load<AudioClip>("SFX/MusicSFX/DinoArea");
        Assert.IsNotNull(testClip, "Test clip not found in Resources/SFX/MusicSFX");
        yield return null;
    }

    [UnityTest]
    public IEnumerator RealVoiceLimit()
    {
        int failureAt = -1;

        for (int voices = 4; voices <= 64; voices += 4)
        {
            var sources = new AudioSource[voices];
            for (int i = 0; i < voices; i++)
            {
                var go = new GameObject("Src_" + i);
                var src = go.AddComponent<AudioSource>();
                src.clip = testClip;
                src.Play();
                sources[i] = src;
            }

            // Let them play a few frames
            for (int f = 0; f < 30; f++) yield return null;

            int realPlaying = 0;
            foreach (var src in sources)
                if (src.isPlaying && src.time > 0f) realPlaying++;

            if (realPlaying < voices)
            {
                failureAt = voices;
                Debug.LogWarning($"Virtualization started at {voices} voices (real={realPlaying})");
                break;
            }

            foreach (var src in sources) Object.Destroy(src.gameObject);
        }

        if (failureAt == -1)
            Assert.Pass("No virtualization detected up to 64 voices");
        else
            Assert.Fail("Virtualization began at " + failureAt + " voices");
    }
}
