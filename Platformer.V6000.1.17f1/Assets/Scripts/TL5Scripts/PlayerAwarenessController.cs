using UnityEngine;

public class PlayerAwarenessController : MonoBehaviour
{
    public bool AwareOfPlayer { get; private set; }
    public Vector2 DirectionToPlayer { get; private set; }

    [SerializeField] private float _playerAwarenessDistance = 6f;

    private Transform _player;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
        else
            Debug.LogError("PlayerAwarenessController: No object with tag 'Player' found!");
    }

    private void Update()
    {
        if (_player == null)
        {
            AwareOfPlayer = false;
            return;
        }

        Vector2 enemyToPlayer = _player.position - transform.position;
        DirectionToPlayer = enemyToPlayer.normalized;
        AwareOfPlayer = enemyToPlayer.magnitude <= _playerAwarenessDistance;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _playerAwarenessDistance);
    }
}
