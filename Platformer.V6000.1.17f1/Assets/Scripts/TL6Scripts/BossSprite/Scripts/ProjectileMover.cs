using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
    public float speed = 10f; // �ӵ��ٶ�
    private float direction; // �ӵ��ƶ�����
    private void Awake()
    {


        // �ӳ������ӵ�
        Destroy(gameObject, 2f);
    }

    // �����ӵ��ƶ�����
    public void SetDirection(float dir)
    {
        direction = dir;
    }

    // Update ÿһ֡����һ��
    void Update()
    {// ���ݷ���ֵ����ת�ӵ��ĳ���
        transform.localScale = new Vector3(direction, transform.localScale.y, transform.localScale.z);
        // �ӵ����ŷ����ƶ�
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