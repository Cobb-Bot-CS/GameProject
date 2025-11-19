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
using UnityEditor;
using System.Net.WebSockets;

public class ActionTest : InputTestFixture
{
    private GameObject character;
    private Animator animator;
    private Rigidbody2D rb;
    private CharacterMove movescript;

    [OneTimeSetUp]
    public override void Setup()
    {
        SceneManager.LoadScene("PlayerTests");

    }

 [UnityTest]
    public IEnumerator ActionTesting()
    {
        var keyboard = InputSystem.AddDevice<Keyboard>();
        InputSystem.Update();

        //Find Character And Goal
        character = GameObject.Find("PlayerCharacter");

        
        //Start Test At Next Platform Down
        character.transform.position = new Vector3(-2.8f, -22.98f, character.transform.position.z);
        Vector3 startpos = character.transform.position;

        //Find Character Components
        rb = character.GetComponent<Rigidbody2D>();
        movescript = character.GetComponent<CharacterMove>();
        animator = character.GetComponent<Animator>();

        int maxIterations = 4;

        for (int i = 0; i < maxIterations; i++)
        {
            yield return new WaitForSeconds(0.5f);
            ResetPosition(startpos);
            InputSystem.Update();

           switch (i)
            {
                case 0:
                    GameObject goal1 = GameObject.Find("Goal(Test2)");
                    var goalTrigger1 = goal1.GetComponent<GoalScript>();

                    //Move Left
                        Press(keyboard.aKey);
                        yield return new WaitForSeconds(2);
                        Release(keyboard.aKey);
                        Assert.IsTrue(goalTrigger1.reachedGoal, "Player did not enter the goal");
                    break;

                case 1:
                    GameObject goal2 = GameObject.Find("Goal(Test3)");
                    var goalTrigger2 = goal2.GetComponent<GoalScript>();

                    //Move Right
                        Press(keyboard.dKey);
                        yield return new WaitForSeconds(2);
                        Release(keyboard.dKey);
                        Assert.IsTrue(goalTrigger2.reachedGoal, "Player did not enter the goal");
                    break;

                case 2:
                    GameObject goal3 = GameObject.Find("Goal(Test4)");
                    var goalTrigger3 = goal3.GetComponent<GoalScript>();

                    //Move Up(Jump)
                        Press(keyboard.spaceKey);
                        yield return new WaitForSeconds(2);
                        Release(keyboard.spaceKey);
                        Assert.IsTrue(goalTrigger3.reachedGoal, "Player did not enter the goal");
                    break;

                case 3:
                    //Attack(Click)
                        var pressState = new MouseState{buttons = 1};
                        InputSystem.QueueStateEvent(Mouse.current,pressState);
                        InputSystem.Update();
                        Assert.IsTrue(animator.GetBool("IsAttacking"), "Character Has Attacked");

                        yield return new WaitForSeconds(1);

                        var releaseState = new MouseState { buttons = 0 };
                        InputSystem.QueueStateEvent(Mouse.current, releaseState);
                        InputSystem.Update();
    
                    break;

                default:
                    Debug.Log("Did Not Do Anything");
                    break;
            }
        }

    }

    private void ResetPosition(Vector3 startpos)
    {
       //Reset Position And Veloctiy
        character.transform.position = startpos;
        rb.linearVelocity = Vector2.zero;
    }
}
 

