using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BoundaryTest_VolumeClamp
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
    public IEnumerator VolumeClampingTest()
    {
        Debug.Log("=== BOUNDARY TEST: Volume Clamping ===");
        
        var go = new GameObject("VolumeTest");
        var src = go.AddComponent<AudioSource>();
        src.clip = testClip;

        // Test 1: Negative volume (out of bounds)
        src.volume = -1.0f;
        yield return null;
        Debug.Log($"Set volume = -1.0, actual = {src.volume}");
        Assert.IsTrue(src.volume >= 0f, "Negative volume should clamp to 0 or higher");

        // Test 2: Volume = 0 (lower boundary)
        src.volume = 0f;
        yield return null;
        Debug.Log($"Set volume = 0.0, actual = {src.volume}");
        Assert.AreEqual(0f, src.volume, 0.01f, "Volume = 0 should be exact");

        // Test 3: Volume = 1 (upper boundary)
        src.volume = 1f;
        yield return null;
        Debug.Log($"Set volume = 1.0, actual = {src.volume}");
        Assert.AreEqual(1f, src.volume, 0.01f, "Volume = 1 should be exact");

        // Test 4: Volume > 1 (out of bounds)
        src.volume = 2.0f;
        yield return null;
        Debug.Log($"Set volume = 2.0, actual = {src.volume}");
        Assert.IsTrue(src.volume <= 1f, "Volume > 1 should clamp to 1 or lower");

        // Test 5: Extreme out of bounds
        src.volume = 100f;
        yield return null;
        Debug.Log($"Set volume = 100.0, actual = {src.volume}");
        Assert.IsTrue(src.volume <= 1f, "Extreme volume should clamp to valid range");

        Object.Destroy(go);
        Assert.Pass("All volume boundary conditions handled correctly");
    }
}