using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinkiiLib
{
    public class JsonSerializer
    {
        public string Serialize<T>(T obj)
        {
            return JsonUtility.ToJson(obj, true);
        }

        public T Deserialize<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }
    }
}
