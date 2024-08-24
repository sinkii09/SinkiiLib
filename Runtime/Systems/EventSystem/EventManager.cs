using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinkiiLib.Systems
{
    public static class EventManager
    {
        private static Dictionary<string, Action<object>> eventDictionary = new Dictionary<string, Action<object>>();

        public static void Addlistener(string eventName, Action<object> listener)
        {
            if(eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
            {
                thisEvent += listener;
                eventDictionary[eventName] = thisEvent;
            }
            else
            {
                thisEvent = listener;
                eventDictionary.Add(eventName, thisEvent);
            }
        }
        public static void Removelistener(string eventName, Action<object> listener)
        {
            if(eventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
            {
                thisEvent -= listener;
                if(thisEvent != null)
                {
                    eventDictionary[eventName] = thisEvent;
                }
                else
                {
                    eventDictionary.Remove(eventName);
                }
            }
        }
        public static void TriggerEvent(string eventName, object eventParam = null)
        {
            if(eventDictionary.TryGetValue(eventName,out Action<object> thisEvent))
            {
                thisEvent?.Invoke(eventParam);
            }
        }
    }
}
