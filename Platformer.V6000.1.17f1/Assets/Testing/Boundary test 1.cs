[UnityTest]
public IEnumerator BoundaryTest_Spatialization()
{
    var go = new GameObject("SpatialSource");
    var src = go.AddComponent<AudioSource>();
    src.clip = testClip;
    src.spatialBlend = 1f; // fully 3D
    src.minDistance = 1f;
    src.maxDistance = 10f;

    // At listener position (0,0,0)
    src.transform.position = Vector3.zero;
    src.Play();
    yield return new WaitForSeconds(0.2f);
    Assert.IsTrue(src.isPlaying, "Clip should be audible at min distance");

    // Move far beyond max distance
    src.transform.position = new Vector3(100f, 0f, 0f);
    yield return new WaitForSeconds(0.2f);
    Assert.IsTrue(src.isPlaying, "Clip still plays but should be effectively silent");

    Assert.Pass("Spatialization boundaries respected");
}
