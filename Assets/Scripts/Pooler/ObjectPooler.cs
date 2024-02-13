using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPooler
{
    public void Init();
}

public class ObjectPooler : MonoBehaviour,  IPooler
{
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public void Init()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            poolDictionary.Add(pool.tag, new Queue<GameObject>());
        }
    }
    public void CreateObject(string tag,Vector2 position, Quaternion rotation)
    {
        if (poolDictionary.ContainsKey(tag))
        {
            GameObject prefab = poolDictionary[tag].Dequeue();
            GameObject obj = GameObject.Instantiate(prefab, position, rotation);
            // do some staff with object
        }
    }

    public void Pull(string tag, Vector2 position, Quaternion rotation)
    {
        if (poolDictionary.ContainsKey(tag))
        {
            GameObject obj = poolDictionary[tag].Dequeue();
            // do some staff with object (set position or etc)
        }
    }
    public void Push(string tag,GameObject obj)
    {
        obj.SetActive(false);
        if (poolDictionary.ContainsKey(tag))
            poolDictionary[tag].Enqueue(obj);
    }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public List<GameObject> prefabs;
        public int size;
    }
}
