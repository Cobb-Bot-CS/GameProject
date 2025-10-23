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

public class SpeedBoundryTest : InputTestFixture
{
    private GameObject character;
    private Rigidbody2D rb;
    private CharacterMove movescript;
    private Vector2 startpos;

    [OneTimeSetUp]
    public override void Setup()
    {
        SceneManager.LoadScene("PlayerTests");

    }

    [UnityTest]
    public IEnumerator SpeedBoundryTesting()
    {
        var keyboard = InputSystem.AddDevice<Keyboard>();

        character = GameObject.Find("PlayerCharacter");

        if (character != null)
        {
            rb = character.GetComponent<Rigidbody2D>();
        }

        if (character != null)
        {
            movescript = character.GetComponent<CharacterMove>();
        }

        float speedamount = 2f;
        startpos = character.transform.position;


        while (character.transform.position.x < 55)
        {
            character.transform.position = startpos;
            //yield return MoveRight(speedamount);
            Press(keyboard.dKey);

            yield return new WaitForSeconds(2);

            Release(keyboard.dKey);

            movescript.IncreaseMoveSpeed(speedamount);
            speedamount = speedamount *10;
            Debug.Log("Speed is Currently: " + speedamount);

            //yield return new WaitForSeconds(2);

        }
    }
    
    private IEnumerator MoveRight(float speed)
    {
        float duration = 3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rb.transform.Translate(Vector2.right * speed * Time.deltaTime);
            elapsed+=Time.deltaTime;
            yield return null;  
        }  
    }
}

