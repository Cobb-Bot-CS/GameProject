using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private CharacterInput mover;

    void Start()
    {
        mover = GetComponent<CharacterInput>();
    }

    void Update()
    {
        Vector2 input = Vector2.zero;
        bool IsClicking = false;

        if (Input.GetKey(KeyCode.W))
        {
            input += Vector2.up;
        }

        if (Input.GetKey(KeyCode.S))
        {
            input += Vector2.down;
        }

        if (Input.GetKey(KeyCode.A))
        {
            input += Vector2.left;
        }

        if (Input.GetKey(KeyCode.D))
        {
            input += Vector2.right;
        }

        if (Input.GetMouseButtonDown(0))
        {
            IsClicking = true;
            //Insert Clicking Function Here
        }

        mover.Move(input);
    }
}
