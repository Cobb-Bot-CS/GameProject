using UnityEngine;

/*
 * Filename: ProjectileMover.cs
 * Developer: Qiwei Liang
 * Purpose: This file is to controll projectile
 */

public class ProjectileMover : MonoBehaviour
{
    public float speed = 10f; 
    private float direction; //+1 right, -1 left
    private void Awake()
    {


        // 2s后销毁子弹
        Destroy(gameObject, 2f);
    }

    //BossAI 在生成子弹时会调用：mover.SetDirection(visualsTransform.localScale.x)，所以子弹方向完全由 Boss 决定

    public void SetDirection(float dir)
    {
        direction = dir;
    }

    // 每帧移动子弹
    void Update()
    {
        // 子弹翻转 sprite 方向，如果 direction = -1 → 子弹水平翻转，让子弹朝着飞的方向面向玩家，不会倒着飞。
        transform.localScale = new Vector3(direction, transform.localScale.y, transform.localScale.z);
        // 子弹按照方向移动。结果： direction = 1 → 子弹右飞
        transform.Translate(Vector2.right * speed * direction * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //  get(CharacterHealthScript)
            CharacterHealth playerHealth = collision.gameObject.GetComponent<CharacterHealth>();

           
            if (playerHealth != null)
            {
                // (CharacterHurt)
                playerHealth.CharacterHurt(10);

                Destroy(gameObject);
            }
            else
            {
  
                Debug.LogError("touched 'Player'but he doesn't have 'CharacterHealth'");
            }
        }
    }
}