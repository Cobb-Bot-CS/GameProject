using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BoundaryTest_Overlapping
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
    public IEnumerator OverlappingOneShots()
    {
        Debug.Log("=== BOUNDARY TEST: Overlapping One-Shots ===");
        
        // Play the same clip multiple times quickly
        for (int i = 0; i < 5; i++)
        {
            AudioSource.PlayClipAtPoint(testClip, Vector3.zero);
            yield return new WaitForSeconds(0.05f); // rapid fire
        }

        // Wait a bit and check that at least one is still playing
        yield return new WaitForSeconds(0.2f);
        var sources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        
        bool anyPlaying = false;
        int playingCount = 0;
        
        foreach (var src in sources)
        {
            if (src.isPlaying)
            {
                anyPlaying = true;
                playingCount++;
            }
        }

        Debug.Log($"Found {sources.Length} audio sources, {playingCount} still playing");

        if (anyPlaying)
            Assert.Pass($"Overlapping one-shots working correctly ({playingCount} sources playing)");
        else
            Assert.Fail("No overlapping one-shots detected - audio may not be playing or cleaned up too quickly");
    }
}