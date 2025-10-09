using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FireBreathDamage : MonoBehaviour
{
    [Tooltip("每秒伤害")]
    public int dps = 60;

    [Tooltip("伤害结算间隔（秒）")]
    public float tickInterval = 0.2f;

    [Tooltip("只对这些层造成伤害（例如勾 Player）")]
    public LayerMask targetLayers;

    Collider2D col;
    readonly Dictionary<Collider2D, float> nextTick = new();

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;

        // 初始关闭，由动画事件开启/关闭
        gameObject.SetActive(false);
    }

    void OnEnable() => nextTick.Clear();
    void OnDisable() => nextTick.Clear();

    void OnTriggerStay2D(Collider2D other)
    {
        // Layer 过滤
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;

        float now = Time.time;
        if (!nextTick.TryGetValue(other, out float readyAt) || now >= readyAt)
        {
            // 你的生命组件名若是 Health，请把 ActorHealth 换成 Health
            var hp = other.GetComponent<Health>();
            if (hp != null)
            {
                int damage = Mathf.RoundToInt(dps * tickInterval);
                hp.Damage(damage);
            }
            nextTick[other] = now + tickInterval;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (nextTick.ContainsKey(other))
            nextTick.Remove(other);
    }
}
