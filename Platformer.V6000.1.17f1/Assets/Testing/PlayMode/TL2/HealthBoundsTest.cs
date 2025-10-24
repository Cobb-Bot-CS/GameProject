using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class HealthBoundsTest : MonoBehaviour
{
    private int damage = 2;
    [SerializeField] private CharacterHealthScript character;

    public void OnTriggerEnter2D(Collider2D collision)
    {
                character.CharacterHurt(damage);
        Debug.Log("Damaged Player For " + damage + " Damage");
        damage *= damage;
    }
}
