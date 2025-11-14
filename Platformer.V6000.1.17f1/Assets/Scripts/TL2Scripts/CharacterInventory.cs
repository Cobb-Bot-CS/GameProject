using UnityEngine;

public class CharacterInventory : MonoBehaviour
{
    //--------------------Character Related References-------------------------//
    [SerializeField] private CharacterState state;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private CharacterAttack playerAttack;

//--------------------Collecting Weapon 1 Function-------------------------//
    public void CollectWeapon1()
    {
        //Update Character State(For Saving Progress)
        state.WeaponWorld1 = true;

        //Update Animator To Include Weapon 1
        playerAnimator.SetBool("HasWeapon1", true);

        //Update Damage Output Accordingly
        playerAttack.typeAttack = 1;
    }
}
