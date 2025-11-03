using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
    public float speed = 10f; // 子弹速度
    private float direction; // 子弹移动方向
    private void Awake()
    {


        // 延迟销毁子弹
        Destroy(gameObject, 2f);
    }

    // 设置子弹移动方向
    public void SetDirection(float dir)
    {
        direction = dir;
    }

    // Update 每一帧调用一次
    void Update()
    {// 根据方向值来翻转子弹的朝向
        transform.localScale = new Vector3(direction, transform.localScale.y, transform.localScale.z);
        // 子弹沿着方向移动
        transform.Translate(Vector2.right * speed * direction * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //  get(CharacterHealthScript)
            CharacterHealthScript playerHealth = collision.gameObject.GetComponent<CharacterHealthScript>();

           
            if (playerHealth != null)
            {
                // (CharacterHurt)
                playerHealth.CharacterHurt(10);

                // 击中后销毁火球
                Destroy(gameObject);
            }
            else
            {
  
                Debug.LogError("touched 'Player'，but he doesn't have 'CharacterHealthScript'");
            }
        }
    }
}