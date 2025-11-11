using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using System.Data;
using UnityEngine.EventSystems;

public class SpeedStressTest : InputTestFixture
{
    private GameObject character;
    private Rigidbody2D rb;
    private CharacterMove movescript;
    private GameObject wall;

    [OneTimeSetUp]
    public override void Setup()
    {
        SceneManager.LoadScene("PlayerTests");

    }

    [UnityTest]
    public IEnumerator SpeedBoundryTesting()
    {
        var keyboard = InputSystem.AddDevice<Keyboard>();

        //Find Character And Wall
        character = GameObject.Find("PlayerCharacter");
        wall = GameObject.Find("Wall");

        //Find Character Components
        rb = character.GetComponent<Rigidbody2D>();
        movescript = character.GetComponent<CharacterMove>();

        //Reset Position
        Vector3 startpos = character.transform.position;
        float speed = 2f;
        int maxIterations = 10;

        for (int i = 0; i < maxIterations; i++)
        {
            //Reset Position And Veloctiy
            character.transform.position = startpos;
            rb.linearVelocity = Vector2.zero;

            //Apply Speed
            movescript.IncreaseMoveSpeed(speed);
            Press(keyboard.dKey);
            yield return new WaitForSeconds(3);
            Release(keyboard.dKey);

            yield return new WaitForSeconds(0.5f);

            float playerX = character.transform.position.x;
            float wallX = wall.transform.position.x;
            float wallWidth = wall.GetComponent<Collider2D>().bounds.extents.x;

            Debug.Log($"Iteration {i + 1}: Speed = {speed}, Player X = {playerX}");

            if (playerX > wallX + wallWidth)
            {
                Debug.Log($"Player clipped through the wall at speed {speed}");
                Assert.Pass();
            }

            speed *= 2f;
        }
        Assert.Fail("Player did not clip through the wall after max speed iteration");
    }
}

