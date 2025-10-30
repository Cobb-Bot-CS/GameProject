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
        
    
       
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(10);
            Destroy(gameObject);
        }
       
    }

}