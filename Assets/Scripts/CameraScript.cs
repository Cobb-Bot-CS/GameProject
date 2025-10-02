using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    void LateUpdate()
    {
        if (target!=null)
        {
            transform.position = target.position + offset;
            transform.LookAt(target);
        }
    }
}
