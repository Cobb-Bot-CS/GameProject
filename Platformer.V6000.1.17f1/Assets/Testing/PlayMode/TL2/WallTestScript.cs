using UnityEngine;

public class HurtTestScript : MonoBehaviour
{
    [SerializeField] private CharacterHealth health;
    private BoxCollider2D hurtBox;

    private int damage = 1;

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D hurtBox)
    {
        damage = (damage + damage) *damage;
        Debug.Log("OUCH! Player Took " + damage + " Damage");
        health.CharacterHurt(damage);
    }
}

