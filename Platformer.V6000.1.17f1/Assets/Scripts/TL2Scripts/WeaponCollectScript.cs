using Unity.VisualScripting;
using UnityEngine;

public class WeaponCollectScript : MonoBehaviour
{
    private BoxCollider2D hitBox;
    [SerializeField] private CharacterInventory inventory;
    [SerializeField] private Animator weaponAnimator;

    void Start()
    {
        hitBox = GetComponent<BoxCollider2D>();
        weaponAnimator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D Collision)
    {
        weaponAnimator.SetBool("IsCollected", true);
        inventory.CollectWeapon1();
        gameObject.SetActive(false);
    }
}
