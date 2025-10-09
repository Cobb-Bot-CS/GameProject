using UnityEngine;

public class CharacterInventory : MonoBehaviour
{
    [SerializeField] private CharacterState state;
    [SerializeField] private Animator playerAnimator;

    public void CollectWeapon1()
    {
        state.WeaponWorld1 = true;
        playerAnimator.SetBool("HasWeapon1", true);
        //Update Damage Output Accordingly
    }
}
