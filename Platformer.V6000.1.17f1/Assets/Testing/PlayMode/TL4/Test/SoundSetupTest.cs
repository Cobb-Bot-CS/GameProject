using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Audio;

public class SoundSetupTest
{
    [UnityTest]
    public IEnumerator TestMixersExist()
    {
        // Test if mixers can be loaded
        var sfxMixer = Resources.Load<AudioMixer>("SFXMixer");
        var musicMixer = Resources.Load<AudioMixer>("MusicMixer");
        var uiMixer = Resources.Load<AudioMixer>("UIMixer");
        var stressMixer = Resources.Load<AudioMixer>("StressTestMixer");
        
        Debug.Log($"SFXMixer: {sfxMixer != null}");
        Debug.Log($"MusicMixer: {musicMixer != null}");
        Debug.Log($"UIMixer: {uiMixer != null}");
        Debug.Log($"StressTestMixer: {stressMixer != null}");
        
        yield return null;
        
        // All should exist since we can see them in your screenshot
        Assert.IsTrue(sfxMixer != null, "SFXMixer should exist");
        Assert.IsTrue(musicMixer != null, "MusicMixer should exist");
        Assert.IsTrue(uiMixer != null, "UIMixer should exist");
        Assert.IsTrue(stressMixer != null, "StressTestMixer should exist");
    }
}