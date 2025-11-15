
using UnityEngine;

public class SoundTriggers : MonoBehaviour
{
    public void PlayerJump() => AudioManager.Instance.Play("Jump");
    public void PlayerFootstep() => AudioManager.Instance.Play("Footstep");
    public void PlayerHurt() => AudioManager.Instance.Play("PlayerHurt");
    public void PlayerDeath() => AudioManager.Instance.Play("PlayerDeath");
    public void PlayerPowerUp() => AudioManager.Instance.Play("PowerUp");
    public void WeaponPickup() => AudioManager.Instance.Play("WeaponPickup");
    public void SwordAttack() => AudioManager.Instance.Play("SwordAttack");
    public void EnemyAttack() => AudioManager.Instance.Play("EnemyAttack");
    public void EnemyDie() => AudioManager.Instance.Play("EnemyDie");
    public void EnemyChase() => AudioManager.Instance.Play("EnemyChase");

    public void CoinPickup() => AudioManager.Instance.Play("CoinPickup");
    public void HealthPickup() => AudioManager.Instance.Play("HealthPickup");

    public void UIButtonClick() => AudioManager.Instance.PlayOneShot("ButtonClick");
    public void PauseOpen() => AudioManager.Instance.Play("PauseOpen");
    public void PauseClose() => AudioManager.Instance.Play("PauseClose");
}
