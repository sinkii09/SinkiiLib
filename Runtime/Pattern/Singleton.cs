using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace SinkiiLib.Pattern
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;
        private static readonly object lockObject = new object();
        public static T Instance 
        { 
            get 
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        try
                        {
                            instance = (T)FindObjectOfType(typeof(T));
                            if (instance == null)
                            {
                                SetupInstance();
                            }
                        }
                        catch (System.Exception ex) 
                        {
                            Debug.LogError($"Error during singleton instance creation: {ex.Message}");
                        }
                    }
                }
                return instance; 
            } 
        }
        public virtual void Awake()
        {
            lock (lockObject)
            {
                RemoveDuplicates();
            }   
        }
        private static void SetupInstance()
        {
            try
            {
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    GameObject gameObj = new GameObject();
                    gameObj.name = typeof(T).Name;
                    instance = gameObj.AddComponent<T>();
                    DontDestroyOnLoad(gameObj);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error in SetupInstance: {ex.Message}");
            }
        }
        private void RemoveDuplicates()
        {
            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = this as T;
                    DontDestroyOnLoad(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
