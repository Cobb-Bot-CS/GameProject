using UnityEngine;

[CreateAssetMenu(menuName = "Boss/BossStats")]
public class BossStats : ScriptableObject
{
    [Header("Basic Info")]
    public string bossName = "Warden";
    public int maxHP = 1000;
    public float moveSpeed = 3f;

    [Header("Poise / Stagger")]
    public int maxPoise = 100;
    public float poiseRecoveryPerSec = 10f;

    [Header("Phase Control")]
    public int phase2TriggerHP = 500;

    [Header("Attack Timing (seconds)")]
    public float slashWindup = 0.5f;
    public float slashActive = 0.2f;
    public float slashCooldown = 0.8f;
}
