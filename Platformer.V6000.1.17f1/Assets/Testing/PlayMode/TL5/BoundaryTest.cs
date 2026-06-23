using NUnit.Framework;
using UnityEngine;

public class BoundaryEnemyTests
{
    GameObject enemy;
    Rigidbody2D rb;

    GameObject player;
    CharacterHealth playerHealth;
    EnemyDamageOnTouch enemyDamage;

    // -----------------------------------------------------
    // SETUP
    // -----------------------------------------------------
    [SetUp]
    public void Setup()
    {
        Physics2D.simulationMode = SimulationMode2D.Script;

        // ----- Player -----
        player = new GameObject("Player");
        player.tag = "Player";
        player.AddComponent<BoxCollider2D>().isTrigger = true;

        playerHealth = player.AddComponent<CharacterHealth>();
        playerHealth.currentHealth = 100;

        player.AddComponent<DummyAnimatorStub>();
        player.AddComponent<CharacterMoveStub>();

        // ----- Enemy -----
        enemy = new GameObject("Enemy");
        rb = enemy.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        enemy.AddComponent<BoxCollider2D>().isTrigger = true;

        enemyDamage = enemy.AddComponent<EnemyDamageOnTouch>();
    }

    [TearDown]
    public void Cleanup()
    {
        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemy);
    }


    // =====================================================
    // DAMAGE BOUNDARY TESTS
    // =====================================================

    [Test]
    public void Boundary_ZeroDamage_NoHealthLoss()
    {
        enemyDamage.damage = 0;
        enemyDamage.SendMessage("OnTriggerEnter2D", player.GetComponent<Collider2D>());
        Assert.AreEqual(100, playerHealth.GetHealth());
    }

    [Test]
    public void Boundary_MaxDamage_HealthClampedZero()
    {
        enemyDamage.damage = 9999;
        enemyDamage.SendMessage("OnTriggerEnter2D", player.GetComponent<Collider2D>());
        Assert.AreEqual(0, playerHealth.GetHealth());
    }


    // =====================================================
    // GRAVITY BOUNDARY TESTS — EXACT MATCH (0, 3, -2)
    // =====================================================

    // Enemy gravity = 0
    [Test]
    public void Boundary_GravityZero_NoVerticalMovement()
    {
        rb.gravityScale = 0;
        enemy.transform.position = new Vector3(0, 5, 0);

        float startY = enemy.transform.position.y;
        Physics2D.Simulate(0.2f);

        Assert.AreEqual(startY, enemy.transform.position.y, 0.01f);
    }

    // Enemy gravity = 3
    [Test]
    public void Boundary_GravityThree_FallsDownFast()
    {
        rb.gravityScale = 3;
        enemy.transform.position = new Vector3(0, 5, 0);

        float startY = enemy.transform.position.y;
        Physics2D.Simulate(0.2f);

        Assert.Less(enemy.transform.position.y, startY);
    }

    // Enemy gravity = –2
    [Test]
    public void Boundary_GravityNegativeTwo_FloatsUp()
    {
        rb.gravityScale = -2;
        enemy.transform.position = new Vector3(0, 5, 0);

        float startY = enemy.transform.position.y;
        Physics2D.Simulate(0.2f);

        Assert.Greater(enemy.transform.position.y, startY);
    }
}


// ------------------------------------------------------------
// REQUIRED STUBS (SAFE, DO NOT CONFLICT WITH YOUR REAL GAME)
// ------------------------------------------------------------
public class DummyAnimatorStub : MonoBehaviour
{
    public void SetBool(string name, bool value) { }
}

public class CharacterMoveStub : MonoBehaviour
{
    public System.Collections.IEnumerator CharacterHurtCooldown() { yield break; }
}
