using UnityEngine;

//--------------------Collect Weapon Script-------------------------//
public class RevisedWeaponCollect : MonoBehaviour
{
    //Reference To Item Hitbox,Character Inventory, Animator, and Pool//
    private BoxCollider2D hitBox;
    [SerializeField] private CharacterInventory inventory;
    [SerializeField] private Animator weaponAnimator;

    [SerializeField] private ObjectPool pool; // Reference to pool manager

//--------------------Establish Components on Startup-------------------------//
    void Start()
    {
        //Grab Item Hitbox
        hitBox = GetComponent<BoxCollider2D>();

        //Grab Animator
        weaponAnimator = GetComponent<Animator>();

        //Grab Pool
        pool = FindFirstObjectByType<ObjectPool>();
    }

//--------------------Collect Weapon On Touch-------------------------//
    void OnTriggerEnter2D(Collider2D collision)
    {
        weaponAnimator.SetBool("IsCollected", true);
        inventory.CollectWeapon1();

        //Return Item To Pool
        pool.ReturnObject(gameObject);
    }
}
