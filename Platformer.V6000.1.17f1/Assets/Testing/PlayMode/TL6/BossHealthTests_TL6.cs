using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class BossHealthTests_TL6
{
    Health bossPrefab;
    BossStats testStats;
    Health bossInstance;

    // Automatically runs *before* each [UnityTest] runs

    [SetUp]
    public void Setup()
    {

        bossPrefab = Resources.Load<Health>("Boss_Warden");
        testStats = Resources.Load<BossStats>("Warden_Stats");

        // If loading fails, fail the test immediately.
        if (bossPrefab == null || testStats == null)
        {
            Assert.Fail("Test Setup Failed: Could not load 'Boss_Warden' prefab or 'Warden_Stats' asset from 'Resources' folder.");
        }
    }


    // Automatically runs *after* each [UnityTest] completes

    [TearDown]
    public void Teardown()
    {
        // Cleans up the Boss instance created during the test
        if (bossInstance != null)
        {
            Object.Destroy(bossInstance.gameObject);
        }
    }


    // Boundary Test 1: "Overkill Test"
    // Rationale: Tests if HP is correctly clamped to 0 when a single instance of damage far exceeds max HP, rather than becoming negative (e.g., -900).

    [UnityTest]
    public IEnumerator BoundaryTest_Overkill_HPClampsToZero()
    {
        // 1. Arrange
        SpawnBoss();
        int overkillDamage = testStats.maxHP + 1000;

        // 2. Act
        bossInstance.Damage(overkillDamage);
        yield return null; // Wait 1 frame to allow Health.cs logic to execute

        // 3. Assert - This is the "clear pass fail" check
        Assert.AreEqual(0, bossInstance.currentHP, "Overkill test failed: HP was not clamped to 0.");
    }


    // Boundary Test 2: "Damage After Death Test"
    // Rationale: Tests if subsequent damage is correctly ignored after the Boss is already dead (HP=0).

    [UnityTest]
    public IEnumerator BoundaryTest_DamageAfterDeath_IsIgnored()
    {
        // Arrange
        SpawnBoss();
        bossInstance.Damage(testStats.maxHP); // First, kill the Boss
        yield return null; // Wait for OnDeath() and isDead=true to execute

        //  Act
        bossInstance.Damage(50); // Attack the Boss again after it is dead
        yield return null;

        // Assert
        Assert.AreEqual(0, bossInstance.currentHP, "Damage after death test failed: HP became negative, damage was not ignored.");
    }






   
    // Stress Test 1: FindPerformanceLimit
    
    //       by gradually increasing Damage() calls until it fails a very strict budget .
  
    
    [UnityTest]
    public IEnumerator StressTest_FindPerformanceLimit()
    {
        // Import stopwatch utility
        var stopwatch = new System.Diagnostics.Stopwatch();

        // This is the budget
        const double FRAME_BUDGET_MS = 0.4;

        int stressLevel = 1; // Start at a success position
        int maxStressToTest = 10000; // A high ceiling for the test

        Debug.Log($"--- [Stress Test] Started: Looking for call count that exceeds {FRAME_BUDGET_MS:F2}ms (Custom Budget) ---");

        while (stressLevel <= maxStressToTest)
        {
            // 1. Arrange (Spawn a new Boss for this stress level)
            SpawnBoss();
            int damagePerHit = 1;
            int expectedHP = Mathf.Max(0, testStats.maxHP - (stressLevel * damagePerHit));

            // --- 2. Act ---

            // Start timer
            stopwatch.Restart();

            // Call Damage() N times in a single frame
            for (int i = 0; i < stressLevel; i++)
            {
                bossInstance.Damage(damagePerHit);
            }

            // Stop timer
            stopwatch.Stop();
            double elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;



            yield return null; // Wait 1 frame

            // --- 3. Assert ---

            // Assert 1: Check if the calculation is still correct
            Assert.AreEqual(expectedHP, bossInstance.currentHP,
                $"Calculation stress test failed at {stressLevel} calls.");

            // Assert 2: Check if performance target was met
            if (elapsedMilliseconds > FRAME_BUDGET_MS)
            {

                // Print the clean message to the Console *before* failing the test
                Debug.Log($"stress test failed at {stressLevel} calls");


                // This will fail the test and report the *detailed* info to the Test Runner
                Assert.Fail($"Performance stress test failed at {stressLevel} calls: " +
                            $"Frame took {elapsedMilliseconds:F2}ms (Budget was {FRAME_BUDGET_MS:F2}ms)");
            }

            // ------------------------

            // --- 4. LOG PROGRESS ---
            // If both Asserts passed, log the success for this stress level
            Debug.Log($"--- [Stress Test] PASSED level: {stressLevel} calls took {elapsedMilliseconds:F2}ms (Budget: {FRAME_BUDGET_MS:F2}ms)");
            // --- END OF LOG ---


            // 5. Cleanup
            Object.Destroy(bossInstance.gameObject);
            bossInstance = null;

            // 6. Gradually increase stress
            if (stressLevel < 100)
                stressLevel += 10;
            else if (stressLevel < 1000)
                stressLevel += 100;
            else if (stressLevel < 10000)
                stressLevel += 1000;
            else
                stressLevel += 5000;
        }

        // If the loop finishes, it's a huge success
        Debug.Log($"--- [Stress Test] PASSED: Performance remained within {FRAME_BUDGET_MS:F2}ms budget even at {maxStressToTest} calls/frame.");
    }

    // --- Helper Methods ---
    void SpawnBoss()
    {
        GameObject bossGO = Object.Instantiate(bossPrefab.gameObject, Vector3.zero, Quaternion.identity);
        bossInstance = bossGO.GetComponent<Health>();
        bossInstance.stats = testStats; // Manually inject dependency
        bossInstance.Awake(); // Manually call Awake to initialize HP

        // --- THIS IS THE FIXED SECTION ---
        // Disable AI to prevent the Boss from moving or consuming CPU during the test
        var controller = bossGO.GetComponent<BossController>();
        if (controller) controller.enabled = false;
        
    }

}