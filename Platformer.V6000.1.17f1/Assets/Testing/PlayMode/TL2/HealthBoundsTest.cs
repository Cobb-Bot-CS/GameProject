using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class HealthBoundsTest
{

    [OneTimeSetUp]
    public void Setup()
    {
        SceneManager.LoadScene("PlayerTests");
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator HealthBoundsTesting()
    {
       
        yield return null;
    }
}
