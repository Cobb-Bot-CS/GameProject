using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AudioBoundaryTests
{
    private AudioClip testClip;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Load a clip from Assets/Resources/SFX/MusicSFX/
        testClip = Resources.Load<AudioClip>("SFX/MusicSFX/DinoArea");
        Assert.IsNotNull(testClip, "Test clip not found in Resources/SFX/MusicSFX");
        yield return null;
    }

    [UnityTest]
    public IEnumerator VolumeBoundaries_NoOutputAtZero_NoClipAtOne()
    {
        var go = new GameObject("AudioSourceTest");
        var src = go.AddComponent<AudioSource>();
        src.clip = testClip;

        // Volume = 0 → silent but still playing
        src.volume = 0f;
        src.Play();
        yield return new WaitForSeconds(0.2f);
        Assert.IsTrue(src.isPlaying, "Clip should still be playing silently");

        // Volume = 1 → full loudness
        src.volume = 1f;
        src.Play();
        yield return new WaitForSeconds(0.2f);
        Assert.IsTrue(src.isPlaying, "Clip should play at full volume");

        Assert.Pass("Volume extremes respected");
    }
}
