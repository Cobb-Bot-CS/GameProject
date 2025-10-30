using UnityEngine;

public class BossMeleeTest : MonoBehaviour
{
    public DamageHitbox meleeHitbox;
    public float windup = 0.2f;  // «∞“°
    public float active = 0.2f;  // …À∫¶÷°
    public float cooldown = 0.5f;// ∫Û“°
    bool busy;

    void Update()
    {
        if (!busy && Input.GetKeyDown(KeyCode.J))
            StartCoroutine(SlashOnce());
    }

    System.Collections.IEnumerator SlashOnce()
    {
        busy = true;
        yield return new WaitForSeconds(windup);
        if (meleeHitbox) meleeHitbox.EnableFor(active); // ø™∆Ùø……À∫¶÷°
        yield return new WaitForSeconds(active + cooldown);
        busy = false;
    }
}

