using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace SinkiiLib.Systems
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance;

        public List<Pool> pools;
        Dictionary<string, ObjectPool<GameObject>> poolDictionary = new Dictionary<string, ObjectPool<GameObject>>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        private void Start()
        {
            foreach (var pool in pools)
            {
                ObjectPool<GameObject> objectPool = new ObjectPool<GameObject>
                (()=> { return Instantiate(pool.prefab); },
                 gameObject => { gameObject.SetActive(true); },
                 gameObject => { gameObject.SetActive(false); },
                 gameObject => { Destroy(gameObject); },
                 false,
                 pool.size
                );
                poolDictionary.Add(pool.tag, objectPool);
            }
        }
        public GameObject SpawnFromPool(string tag)
        {
            if(!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return null;
            }
            GameObject objectToSpawn = poolDictionary[tag].Get();
            return objectToSpawn;
        }
        public void ReleaseGameObject(string tag, GameObject gameObject)
        {
            poolDictionary[tag].Release(gameObject);
        }
    }
    [Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
}
