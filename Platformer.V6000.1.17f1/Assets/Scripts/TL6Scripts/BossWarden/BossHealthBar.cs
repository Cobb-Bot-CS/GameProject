using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [Header("Bindings")]
    public Slider slider;            // 拖 BossHealthBar 自己的 Slider
    public Transform bossRoot;       // 只拖 Boss（例如 Boss_Warden）这个对象

    [Header("Display")]
    public Vector3 worldOffset = new Vector3(0, 2f, 0); // 血条在 Boss 头顶的偏移

    private Camera cam;
    private Health bossHealth;       // 只会从 bossRoot 上查找

    void Awake()
    {
        cam = Camera.main;
        if (bossRoot != null)
        {
            // 只从 bossRoot（及其子物体）上找 Health，避免拿到玩家的
            bossHealth = bossRoot.GetComponentInChildren<Health>();
            if (bossHealth == null)
            {
                Debug.LogError("[BossHealthUI] 未在 bossRoot 上找到 Health 组件，请检查 Boss 对象。");
            }
        }
        else
        {
            Debug.LogError("[BossHealthUI] bossRoot 未指定。");
        }
    }

    void Start()
    {
        if (bossHealth != null && slider != null)
        {
            // 注意：Health.cs 用的是 playerMaxHP 作为最大生命字段（通用但命名容易误解）:contentReference[oaicite:1]{index=1}
            slider.minValue = 0;
            slider.maxValue = bossHealth.playerMaxHP;
            slider.value = bossHealth.currentHP;
        }
    }

    void Update()
    {
        if (bossHealth == null || slider == null || cam == null) return;

        // 运行期保持同步
        slider.value = bossHealth.currentHP;

        // 跟随 Boss 头顶
        Vector3 screenPos = cam.WorldToScreenPoint(bossRoot.position + worldOffset);
        transform.position = screenPos;

        // Boss 死亡时隐藏（可选）
        if (bossHealth.currentHP <= 0 && gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
