using UnityEngine;

public class BossArenaController : MonoBehaviour
{
    [Header("Boss 引用")]
    [Tooltip("将场景中的Boss角色拖拽到这里")]
    [SerializeField] private BossAI_Advanced bossAI;

    private void Start()
    {
        if (bossAI == null)
        {
            Debug.LogError("BossArenaController 没有指定 BossAI 的引用！", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 确保进入的是玩家，并且玩家有 "Player" 标签
        if (other.CompareTag("Player"))
        {
            // 告诉Boss：你有目标了！
            bossAI.EngageTarget(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 告诉Boss：你的目标跑了！
            bossAI.DisengageTarget();
        }
    }
}