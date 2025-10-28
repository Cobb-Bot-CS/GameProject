using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class HealthBoundsTest : MonoBehaviour
{
    private GameObject character;
    private CharacterHealthScript characterHealth;

    [UnityTest]
        public IEnumerator TestPositiveDamage()
    {
        SceneManager.LoadScene("PlayerTests");
        yield return new WaitForSeconds(1f);

        character = GameObject.Find("PlayerCharacter");
        characterHealth = character.GetComponent<CharacterHealthScript>();         
        characterHealth.currentHealth = 100;       
       
        characterHealth.CharacterHurt(10);
        Assert.AreEqual(90, characterHealth.GetHealth());
    }

    [UnityTest]
    public IEnumerator TestZeroDamage()
    {
        SceneManager.LoadScene("PlayerTests");
        yield return new WaitForSeconds(1f);

        character = GameObject.Find("PlayerCharacter");
        characterHealth = character.GetComponent<CharacterHealthScript>();
        characterHealth.currentHealth = 100;
       
        characterHealth.CharacterHurt(0);
        Assert.AreEqual(100, characterHealth.GetHealth());
    }

    [UnityTest]
    public IEnumerator TestNegativeDamage()
    {
        SceneManager.LoadScene("PlayerTests");
        yield return new WaitForSeconds(1f);

        character = GameObject.Find("PlayerCharacter");
        characterHealth = character.GetComponent<CharacterHealthScript>();
        characterHealth.currentHealth = 100;
       
        characterHealth.CharacterHurt(-5);
        Assert.AreEqual(100, characterHealth.GetHealth());
    }
}
