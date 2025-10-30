using UnityEngine;

public class BossArenaController : MonoBehaviour
{
    [Header("Boss ����")]
    [Tooltip("�������е�Boss��ɫ��ק������")]
    [SerializeField] private BossAI_Advanced bossAI;

    private void Start()
    {
        if (bossAI == null)
        {
            Debug.LogError("BossArenaController û��ָ�� BossAI �����ã�", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ȷ�����������ң���������� "Player" ��ǩ
        if (other.CompareTag("Player"))
        {
            // ����Boss������Ŀ���ˣ�
            bossAI.EngageTarget(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ����Boss�����Ŀ�����ˣ�
            bossAI.DisengageTarget();
        }
    }
}