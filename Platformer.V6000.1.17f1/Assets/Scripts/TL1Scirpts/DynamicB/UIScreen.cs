using UnityEngine;

public class UIScreen : MonoBehaviour
{
    public virtual void Show()
    {
        Debug.Log("Showing a generic UI screen.");
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        Debug.Log("Hiding a generic UI screen.");
        gameObject.SetActive(false);
    }
}
