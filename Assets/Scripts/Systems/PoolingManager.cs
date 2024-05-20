using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using System;

public class PoolingManager : Singleton<PoolingManager>
{
    public override void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this);
        base.Awake();

    }
    public List<ObjectPool> poolGameObjects = new List<ObjectPool>();
    public T GetGameObject<T>(GameObject prefab)
    {
        ObjectPool pool = poolGameObjects.Find((obj) => { return obj.prefab == prefab; });
        if (pool == null)
        {
            ObjectPool newPool = new ObjectPool(() => Instantiate(prefab));
            newPool.prefab = prefab;
            poolGameObjects.Add(newPool);
            pool = newPool;
        }
        return pool.Pop().GetComponent<T>();
    }

    public T GetGameObject<T>(GameObject prefab, Transform parent)
    {
        ObjectPool pool = poolGameObjects.Find((obj) => { return obj.prefab == prefab; });
        if (pool == null)
        {
            ObjectPool newPool = new ObjectPool(() => Instantiate(prefab, parent));
            newPool.prefab = prefab;
            poolGameObjects.Add(newPool);
            pool = newPool;
        }
        GameObject obj = pool.Pop();
        obj.transform.SetParent(parent);
        obj.transform.localPosition = Vector3.zero;
        return obj.GetComponent<T>();
    }

    public void DisablePool(GameObject prefab)
    {
        ObjectPool pool = poolGameObjects.Find((obj) => { return obj.prefab == prefab; });
        if (pool == null)
        {
            return;
        }

        for (int i = 0; i < pool.objectPool.Count; i++)
        {
            if (pool.objectPool[i] != null)
                pool.objectPool[i].gameObject.SetActive(false);

        }

    }
    public int GetNumberActiveObject(GameObject prefab)
    {
        ObjectPool pool = poolGameObjects.Find((obj) => { return obj.prefab == prefab; });
        if (pool == null)
        {
            return 0;
        }
        int count = 0;
        for (int i = 0; i < pool.objectPool.Count; i++)
        {
            if (pool.objectPool[i] != null && pool.objectPool[i].gameObject.activeSelf)
            {
                count++;
            }
        }
        return count;
    }

    public List<T> GetActiveObject<T>(GameObject prefab)
    {
        ObjectPool pool = poolGameObjects.Find((obj) => { return obj.prefab == prefab; });
        if (pool == null)
        {
            return null;
        }

        List<T> objects = new List<T>();
        for (int i = pool.objectPool.Count - 1; i >= 0; i--)
        {
            if (pool.objectPool[i] == null)
            {
                pool.objectPool.RemoveAt(i);
                continue;
            }

            if (pool.objectPool[i].gameObject.activeSelf)
            {
                objects.Add(pool.objectPool[i].GetComponent<T>());
            }
        }
        return objects;
    }
}

[System.Serializable]
public class ObjectPool
{
    public GameObject prefab;
    public List<GameObject> objectPool = new List<GameObject>();
    private readonly Func<GameObject> createAction;
    public ObjectPool(Func<GameObject> createAction)
    {
        this.createAction = createAction;
    }
    public GameObject Pop()
    {
        for (int i = objectPool.Count - 1; i >= 0; i--)
        {
            if (objectPool[i] == null)
            {
                objectPool.RemoveAt(i);
                continue;
            }

            if (!objectPool[i].gameObject.activeSelf)
            {
                return objectPool[i];
            }
        }
        GameObject newObj = createAction();
        objectPool.Add(newObj);
        return newObj;

    }

}

