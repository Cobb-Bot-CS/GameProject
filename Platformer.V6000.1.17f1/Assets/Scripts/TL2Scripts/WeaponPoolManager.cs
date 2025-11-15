using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    //References Weapon Prefab And Sets Pool To Contain X Size//
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 1; //Dont need multiple

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        // Establish Objects On Startup
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

//--------------------Collect Object From Pool-------------------------//
    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = Instantiate(prefab); // Expand pool if needed
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

//--------------------Return Object To Pool-------------------------//
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
