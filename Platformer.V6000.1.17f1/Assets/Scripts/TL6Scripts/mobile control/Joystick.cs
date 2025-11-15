using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private RectTransform bg;
    private RectTransform handle;
    private Vector2 inputVector;

    //  ��������¼�Ƿ񴥷���Ծ
    private bool jumpTriggered = false;
    [Header("Jump Settings")]
    public float jumpThreshold = 0.6f;  // �϶�����㡰��Ծ��

    void Start()
    {
        bg = GetComponent<RectTransform>();
        handle = transform.GetChild(0).GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            bg, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x /= (bg.sizeDelta.x / 2f);
            pos.y /= (bg.sizeDelta.y / 2f);

            inputVector = new Vector2(pos.x, pos.y);
            if (inputVector.magnitude > 1f)
                inputVector = inputVector.normalized;

            handle.anchoredPosition = new Vector2(
                inputVector.x * (bg.sizeDelta.x / 3f),
                inputVector.y * (bg.sizeDelta.y / 3f)
            );

            //  ���������Ծ
            if (inputVector.y > jumpThreshold && !jumpTriggered)
            {
                jumpTriggered = true;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        jumpTriggered = false;
    }

    public float Horizontal() => inputVector.x;
    public float Vertical() => inputVector.y;

    // ��������������ҽű�������Ծ�ź�
    public bool JumpTriggered()
    {
        if (jumpTriggered)
        {
            jumpTriggered = false; // ����״̬
            return true;
        }
        return false;
    }
}
