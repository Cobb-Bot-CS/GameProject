[UnityTest]
public IEnumerator StressTest_FrameStability()
{
    int failureAt = -1;

    for (int voices = 8; voices <= 64; voices += 8)
    {
        for (int i = 0; i < voices; i++)
            AudioSource.PlayClipAtPoint(testClip, Vector3.zero);

        float spikeThresholdMs = 25f;
        int spikeFrames = 0;

        for (int f = 0; f < 120; f++) // ~2 seconds at 60fps
        {
            if (Time.deltaTime * 1000f > spikeThresholdMs)
                spikeFrames++;
            yield return null;
        }

        if (spikeFrames > 10) // sustained spikes
        {
            failureAt = voices;
            Debug.LogWarning($"Frame instability at {voices} voices");
            break;
        }
    }

    if (failureAt == -1)
        Assert.Pass("No frame instability detected up to 64 voices");
    else
        Assert.Fail("Frame instability began at " + failureAt + " voices");
}
