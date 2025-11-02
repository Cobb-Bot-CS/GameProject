using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAwarenessController))]
public class EnemyMovement2D : MonoBehaviour
{
    [SerializeField] private float _speed = 2f;
    [SerializeField] private bool _flipSprite = true;

    private Rigidbody2D _rb;
    private PlayerAwarenessController _awareness;
    private SpriteRenderer _sr;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _awareness = GetComponent<PlayerAwarenessController>();
        _sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (_awareness == null)
            return;

        if (_awareness.AwareOfPlayer)
        {
            float dirX = Mathf.Sign(_awareness.DirectionToPlayer.x);
            _rb.linearVelocity = new Vector2(dirX * _speed, _rb.linearVelocity.y);

            if (_flipSprite && _sr != null && dirX != 0)
                _sr.flipX = dirX < 0;
        }
        else
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
        }
    }
}
