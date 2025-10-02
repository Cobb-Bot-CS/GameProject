using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private float speed = 3f;

    public void Move(Vector2 direction)
    {
        Vector3 movement = new Vector3(direction.x, 0, direction.y) * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);
    }
}
