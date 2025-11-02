using UnityEngine;

public class ArenaController : MonoBehaviour
{
    [Tooltip("可选：直接把Boss的GameObject拖进来；留空则自动寻找场景里第一个带BossWardenAI脚本的对象。")]
    public GameObject bossObject;

    private Transform player;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player")?.transform;

        if (bossObject == null)
        {
            // 尝试自动找带有脚本名“BossWardenAI”的物体
            var all = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (var go in all)
            {
                if (go.GetComponent("BossWardenAI") != null)
                {
                    bossObject = go;
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (bossObject != null)
        {
            // ✅ 不再强依赖类型，避免 CS1061
            bossObject.SendMessage("ActivateBattle", SendMessageOptions.DontRequireReceiver);
            Debug.Log("[Arena] Player entered, ActivateBattle sent.");
        }
        else
        {
            Debug.LogWarning("[Arena] bossObject is null.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (bossObject != null)
        {
            bossObject.SendMessage("DeactivateBattle", SendMessageOptions.DontRequireReceiver);
            Debug.Log("[Arena] Player left, DeactivateBattle sent.");
        }
    }
}
