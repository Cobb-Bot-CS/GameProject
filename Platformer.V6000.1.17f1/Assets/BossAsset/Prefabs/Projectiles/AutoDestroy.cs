using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [Tooltip("��������١��������ʱ����֡�� / Samples����")]
    public float life = 0.5f;

    [Tooltip("�����ѡ�������ٸ����壨������Ч������������ʱ����")]
    public bool destroyParent = false;

    void OnEnable()
    {
        var target = destroyParent && transform.parent ? transform.parent.gameObject : gameObject;
        if (life <= 0f) life = 0.5f; // ����
        Destroy(target, life);
    }

    // �������¼����ã���ѡ�����ڶ������һֱ֡�ӵ���
    public void DestroyNow()
    {
        var target = destroyParent && transform.parent ? transform.parent.gameObject : gameObject;
        Destroy(target);
    }
}
