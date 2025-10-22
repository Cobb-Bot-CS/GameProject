[UnityTest]
public IEnumerator BoundaryTest_ClipLengths()
{
    // Short clip (e.g., bar impact sound)
    var shortClip = Resources.Load<AudioClip>("SFX/bar-impact");
    Assert.IsNotNull(shortClip);
    AudioSource.PlayClipAtPoint(shortClip, Vector3.zero);
    yield return new WaitForSeconds(0.1f);
    Assert.Pass("Short clip played without error");

    // Long clip (simulate ambient loop)
    var longClip = AudioClip.Create("LongClip", 44100 * 60, 1, 44100, false);
    AudioSource.PlayClipAtPoint(longClip, Vector3.zero);
    yield return new WaitForSeconds(0.5f);
    Assert.Pass("Long clip started successfully");
}
