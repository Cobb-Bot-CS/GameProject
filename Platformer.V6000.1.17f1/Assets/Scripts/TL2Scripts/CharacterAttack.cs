using System.Collections;
using Codice.Client.Common.GameUI;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{

    [SerializeField] private CharacterMove clickDetection;
    [SerializeField] private CapsuleCollider2D weaponHitbox;
    [SerializeField] private Animator animator;
    [SerializeField] private BossAI_Advanced bossScript;
    [SerializeField] private CapsuleCollider2D bossHitbox;
    [SerializeField] private int baseDamage = 5;
    [SerializeField] private int weapon1Damage = 10;

    private int typeAttack = 0;
    
    

    public IEnumerator Attack()
    {
        animator.SetBool("IsAttacking", true);
        if (weaponHitbox.IsTouching(bossHitbox))
        {
            if (animator.GetBool("HasWeapon1") == true)
            {
                typeAttack = 1;
            }

            switch (typeAttack)
            {
                case 1:
                    bossScript.TakeDamage(weapon1Damage);
                    break;
                default:
                    bossScript.TakeDamage(baseDamage);
                    break;
            }

        }

        yield return new WaitForSeconds(0.3f);
        animator.SetBool("IsAttacking", false);

    }
}
