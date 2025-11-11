/*
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class BoundaryTests
{
    private SettingsManager settingsManager;
    private Text bcModeText;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Create a temporary GameObject for SettingsManager
        GameObject go = new GameObject("SettingsManager");
        settingsManager = go.AddComponent<SettingsManager>();

        // Create a temporary Text object for BC Mode
        GameObject textGO = new GameObject("BCModeText");
        bcModeText = textGO.AddComponent<Text>();
        bcModeText.gameObject.SetActive(false);

        // Assign to SettingsManager
        settingsManager.bcModeStatusText = bcModeText;

        yield return null;
    }

    [UnityTest]
    public IEnumerator BCMode_EnablesText_WhenTrue()
    {
        settingsManager.ToggleBCMode(true);
        yield return null;

        Assert.IsTrue(settingsManager.BCMode);
        Assert.IsTrue(bcModeText.gameObject.activeSelf);
        Assert.AreEqual("BC MODE ACTIVE", bcModeText.text);
    }

    [UnityTest]
    public IEnumerator BCMode_DisablesText_WhenFalse()
    {
        settingsManager.ToggleBCMode(false);
        yield return null;

        Assert.IsFalse(settingsManager.BCMode);
        Assert.IsFalse(bcModeText.gameObject.activeSelf);
    }
}
*/