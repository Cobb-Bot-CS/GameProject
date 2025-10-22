using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AudioBoundaryTests
{
    [UnityTest]
    public IEnumerator BoundaryTest_ShortClip()
    {
        // Short clip (e.g., bar impact sound)
        var shortClip = Resources.Load<AudioClip>("SFX/bar-impact");
        Assert.IsNotNull(shortClip, "Short clip not found in Resources/SFX");

        AudioSource.PlayClipAtPoint(shortClip, Vector3.zero);
        yield return new WaitForSeconds(0.1f);

        Assert.Pass("Short clip played without error");
    }

    [UnityTest]
    public IEnumerator BoundaryTest_LongClip()
    {
        // Long clip (simulate ambient loop, 60 seconds at 44.1kHz mono)
        var longClip = AudioClip.Create("LongClip", 44100 * 60, 1, 44100, false);
        Assert.IsNotNull(longClip, "Long clip creation failed");

        AudioSource.PlayClipAtPoint(longClip, Vector3.zero);
        yield return new WaitForSeconds(0.5f);

        Assert.Pass("Long clip started successfully");
    }
}
