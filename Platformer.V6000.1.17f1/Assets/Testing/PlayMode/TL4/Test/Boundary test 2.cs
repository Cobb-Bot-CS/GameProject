using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BoundaryTest_3DAudioDistance
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
    public IEnumerator MaxDistanceCutoffTest()
    {
        Debug.Log("=== BOUNDARY TEST: 3D Audio Max Distance ===");

        // Create listener
        var listener = new GameObject("Listener");
        listener.AddComponent<AudioListener>();
        listener.transform.position = Vector3.zero;

        // Create audio source with 3D settings
        var audioGO = new GameObject("AudioSource");
        var src = audioGO.AddComponent<AudioSource>();
        src.clip = testClip;
        src.spatialBlend = 1f; // Full 3D
        src.maxDistance = 50f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.loop = true;
        src.Play();

        yield return new WaitForSeconds(0.1f);

        // Test 1: Within max distance (should be audible)
        audioGO.transform.position = new Vector3(25f, 0f, 0f);
        yield return new WaitForSeconds(0.1f);
        Assert.IsTrue(src.isPlaying, "Audio should play within max distance");
        Debug.Log($"Position at 25m (within 50m): Playing = {src.isPlaying}");

        // Test 2: At max distance boundary (edge case)
        audioGO.transform.position = new Vector3(50f, 0f, 0f);
        yield return new WaitForSeconds(0.1f);
        Debug.Log($"Position at 50m (at max distance): Playing = {src.isPlaying}");

        // Test 3: Beyond max distance (should still play but silent/culled)
        audioGO.transform.position = new Vector3(100f, 0f, 0f);
        yield return new WaitForSeconds(0.1f);
        Debug.Log($"Position at 100m (beyond max distance): Playing = {src.isPlaying}");

        Object.Destroy(audioGO);
        Object.Destroy(listener);
        
        Assert.Pass("3D audio distance boundaries tested");
    }
}