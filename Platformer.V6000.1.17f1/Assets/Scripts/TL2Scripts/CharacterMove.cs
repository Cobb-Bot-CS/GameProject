using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    void Update()
    {
        Vector2 move = Vector2.zero;

        if(Input.GetKey(KeyCode.A))
        {
            move += Vector2.left;
        }

        if(Input.GetKey(KeyCode.D))
        {
            move += Vector2.right;
        }

        transform.Translate(move * moveSpeed *Time.deltaTime);
    }
}
