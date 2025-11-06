using UnityEngine;

public class EnemyAttackToCharacterHurt : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 1.5f;      // how close the player has to be
    public int damage = 10;               // how much to hurt the player
    public float attackCooldown = 1f;     // time between hits

    private float _nextAttackTime = 0f;

    private Transform _player;
    private CharacterHealthScript _playerHealth;

    void Start()
    {
        // find player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
            _playerHealth = playerObj.GetComponent<CharacterHealthScript>();
        }
        else
        {
            Debug.LogError("EnemyAttackToCharacterHurt: No object with tag 'Player' found!");
        }
    }

    void Update()
    {
        if (_player == null || _playerHealth == null)
            return;

        // how far is player
        float dist = Vector2.Distance(transform.position, _player.position);

        // close enough + cooldown over → hit
        if (dist <= attackRange && Time.time >= _nextAttackTime)
        {
            AttackPlayer();
            _nextAttackTime = Time.time + attackCooldown;
        }
    }

    void AttackPlayer()
    {
        //  call the method YOUR player has
        _playerHealth.CharacterHurt(damage);
        Debug.Log($"{name} hit player for {damage}");
    }

    // just to see the circle
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
