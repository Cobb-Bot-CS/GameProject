#if UNITY_EDITOR
using Codice.Client.Common.GameUI;
#endif
using System.Collections;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private CharacterMove clickDetection;

//--------------------Reference Character Variables-------------------------//
    [SerializeField] private CapsuleCollider2D weaponHitbox;
    [SerializeField] private Animator animator;

//--------------------Reference Boss Variables-------------------------//
    [SerializeField] private BossAI_Advanced bossScript;
    [SerializeField] private CapsuleCollider2D bossHitbox;

    //--------------------Weapons To Damage Scale (Better Weapons = More Damage-------------------------//
    [SerializeField] private float baseDamage = 5;
    [SerializeField] private float weapon1Damage = 10;

    //Distinguish Between Weapons Gathered Via Type Counter//
    public int typeAttack = 0;

//--------------------Attack Function-------------------------//
    public IEnumerator Attack()
    {
        AudioManager.Instance.Play("SwordAttack");
        //Contact Animator To Change Animation
        animator.SetBool("IsAttacking", true);

        //Checks if Hitbox is Touching Before Implementing Damage.
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

        //CoolDown To Prevent Spam Attacking
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("IsAttacking", false);
    }
}
