[UnityTest]
public IEnumerator BoundaryTest_LoopingVsOneShot()
{
    // One-shot
    var go1 = new GameObject("OneShot");
    var src1 = go1.AddComponent<AudioSource>();
    src1.clip = testClip;
    src1.loop = false;
    src1.Play();
    yield return new WaitForSeconds(testClip.length + 0.1f);
    Assert.IsFalse(src1.isPlaying, "One-shot should stop after clip length");

    // Looping
    var go2 = new GameObject("Looping");
    var src2 = go2.AddComponent<AudioSource>();
    src2.clip = testClip;
    src2.loop = true;
    src2.Play();
    yield return new WaitForSeconds(testClip.length * 2);
    Assert.IsTrue(src2.isPlaying, "Looping clip should still be playing");

    Assert.Pass("Looping and one-shot boundaries verified");
}
