using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FireBreathDamage : MonoBehaviour
{
    [Tooltip("ÿ���˺�")]
    public int dps = 60;

    [Tooltip("�˺����������룩")]
    public float tickInterval = 0.2f;

    [Tooltip("ֻ����Щ������˺������繴 Player��")]
    public LayerMask targetLayers;

    Collider2D col;
    readonly Dictionary<Collider2D, float> nextTick = new();

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;

        // ��ʼ�رգ��ɶ����¼�����/�ر�
        gameObject.SetActive(false);
    }

    void OnEnable() => nextTick.Clear();
    void OnDisable() => nextTick.Clear();

    void OnTriggerStay2D(Collider2D other)
    {
        // Layer ����
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;

        float now = Time.time;
        if (!nextTick.TryGetValue(other, out float readyAt) || now >= readyAt)
        {
            // ���������������� Health����� ActorHealth ���� Health
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
