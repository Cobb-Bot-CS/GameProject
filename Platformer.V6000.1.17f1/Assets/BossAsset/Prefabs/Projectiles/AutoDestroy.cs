using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [Tooltip("几秒后销毁。建议填动画时长（帧数 / Samples）。")]
    public float life = 0.5f;

    [Tooltip("如果勾选，则销毁父物体（用于特效做在子物体上时）。")]
    public bool destroyParent = false;

    void OnEnable()
    {
        var target = destroyParent && transform.parent ? transform.parent.gameObject : gameObject;
        if (life <= 0f) life = 0.5f; // 兜底
        Destroy(target, life);
    }

    // 供动画事件调用（可选）：在动画最后一帧直接调它
    public void DestroyNow()
    {
        var target = destroyParent && transform.parent ? transform.parent.gameObject : gameObject;
        Destroy(target);
    }
}
