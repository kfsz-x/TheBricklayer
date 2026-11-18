using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityCore
{
    public sealed class PrefabPool : IDisposable
    {
        private readonly Transform _parent;
        private readonly Dictionary<GameObject, Stack<GameObject>> _idle = new();
        private readonly Dictionary<GameObject, GameObject> _active = new();
        private bool _disposed;

        public PrefabPool(Transform parent)
        {
            _parent = parent;
        }

        public GameObject Rent(GameObject prefab)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(PrefabPool));
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            GameObject instance = null;
            if (_idle.TryGetValue(prefab, out var instances))
            {
                while (instance == null && instances.Count > 0)
                    instance = instances.Pop();
            }

            if (instance == null)
                instance = Object.Instantiate(prefab, _parent);

            // Instantiating a prefab can call user code before the clone is tracked.
            if (_disposed)
            {
                DestroyInstance(instance);
                throw new ObjectDisposedException(nameof(PrefabPool));
            }

            _active.Add(instance, prefab);
            instance.SetActive(true);
            return instance;
        }

        public bool Return(GameObject instance)
        {
            if (_disposed || ReferenceEquals(instance, null) || !_active.TryGetValue(instance, out var prefab))
                return false;

            _active.Remove(instance);
            if (instance == null)
                return false;

            instance.SetActive(false);
            if (instance != null && !_disposed)
                instance.transform.SetParent(_parent, false);

            if (_disposed || prefab == null)
            {
                DestroyInstance(instance);
            }
            else if (instance != null)
            {
                if (!_idle.TryGetValue(prefab, out var instances))
                {
                    instances = new Stack<GameObject>();
                    _idle.Add(prefab, instances);
                }

                instances.Push(instance);
            }

            return true;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            foreach (var instance in _active.Keys)
                DestroyInstance(instance);
            foreach (var instances in _idle.Values)
                foreach (var instance in instances)
                    DestroyInstance(instance);

            _active.Clear();
            _idle.Clear();
        }

        private static void DestroyInstance(GameObject instance)
        {
            if (instance == null)
                return;

            if (Application.isPlaying)
                Object.Destroy(instance);
            else
                Object.DestroyImmediate(instance);
        }
    }
}
