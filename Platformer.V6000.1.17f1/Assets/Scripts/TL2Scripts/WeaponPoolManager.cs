using System.Collections.Generic;
using UnityEngine;

//--------------------Super-Class-------------------------//
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

    // Virtual Function
    public virtual GameObject GetObject(Vector3 position, Quaternion rotation)
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

    // Non-virtual function -> statically bound method
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}


//--------------------Sub-Class-------------------------//
public class SpecialObjectPool : ObjectPool
{
    //Override the dynamically bound method
    public override GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        Debug.Log("SpecialObjectPool: spawning with extra effects!");
        GameObject obj = base.GetObject(position, rotation);
        // Add custom behavior here
        return obj;
    }
}

//ObjectPool pool;                ==  Static Type Object Pool
//ObjectPool pool = new SpecialObjectPool()  ==  Dynamic Type Object Pool

// Dynamic binding only shows when using inheritance and virtual methods.

// Dynamically bound method: GetObject() → Calls SpecialObjectPool.GetObject() 
// if dynamic type is SpecialObjectPool; calls ObjectPool.GetObject() if dynamic 
// type is ObjectPool.
