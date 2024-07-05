using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spans.Skeleton
{
    public class EventBus : MonoBehaviour
    {
        private static EventBus _instance;
        public static EventBus Instance
        {
            get
            {
                if (_instance == null)
                {
                    var eventBusGameObject = new GameObject("EventBus");
                    _instance = eventBusGameObject.AddComponent<EventBus>();
                    DontDestroyOnLoad(eventBusGameObject);
                }
                return _instance;
            }
        }

        private readonly Dictionary<Type, List<Action<object>>> _eventListeners = new Dictionary<Type, List<Action<object>>>();

        public void Register<T>(Action<T> listener) where T : class
        {
            Type eventType = typeof(T);
            if (!_eventListeners.ContainsKey(eventType))
            {
                _eventListeners[eventType] = new List<Action<object>>();
            }
            _eventListeners[eventType].Add(obj => listener(obj as T));
        }

        public void Unregister<T>(Action<T> listener) where T : class
        {
            Type eventType = typeof(T);
            if (_eventListeners.TryGetValue(eventType, out var eventListener))
            {
                eventListener.Remove(obj => listener(obj as T));
            }
        }

        public void Trigger<T>(T eventData) where T : class
        {
            Type eventType = typeof(T);
            if (_eventListeners.TryGetValue(eventType, out var eventListeners))
            {
                foreach (var listener in eventListeners)
                {
                    listener(eventData);
                }
            }
        }
    }
}