using UnityEngine;

/*
 * Filename: BossArenaController.cs
 * Developer: Qiwei Liang
 * Purpose: This file is to identify whether player is in arena
 */


public class BossArenaController : MonoBehaviour
{
    [Header("Boss 引用")]
    [Tooltip("将场景中的Boss角色拖拽到这里")]  //Tooltip：鼠标悬停提示
    [SerializeField] private BossAI_Advanced bossAI;

    private void Start()
    {
        if (bossAI == null)
        {
            //如果你忘了拖 BossAI，会报错提醒你
            Debug.LogError("BossArenaController There is no reference to BossAI!", this);
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