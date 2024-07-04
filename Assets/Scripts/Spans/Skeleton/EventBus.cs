using System;
using System.Collections.Generic;

namespace Spans.Skeleton
{
    public class EventBus
    {
        private Dictionary<Type, List<Action<object>>> _eventListeners = new Dictionary<Type, List<Action<object>>>();

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
            if (_eventListeners.ContainsKey(eventType))
            {
                _eventListeners[eventType].Remove(obj => listener(obj as T));
            }
        }

        public void Trigger<T>(T eventData) where T : class
        {
            Type eventType = typeof(T);
            if (_eventListeners.TryGetValue(eventType, out var eventListener))
            {
                foreach (var listener in eventListener)
                {
                    listener(eventData);
                }
            }
        }
    }
}