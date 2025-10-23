using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageHitbox : MonoBehaviour
{
    public int damage = 20;
    public LayerMask targetLayers;
    private Collider2D col;

    void Awake() { col = GetComponent<Collider2D>(); col.isTrigger = true; col.enabled = false; }

    public void EnableFor(float seconds) => StartCoroutine(DoEnable(seconds));

    System.Collections.IEnumerator DoEnable(float t)
    {
        col.enabled = true;
        yield return new WaitForSeconds(t);
        col.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;
        var hp = other.GetComponent<Health>();
        if (hp) hp.Damage(damage);
    }
}
