using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IdeaToGame.ObjectPooling
{
    public static class ObjectPool
    {
        public static event Action OnPoolCleared = delegate { };
        
        private static readonly Dictionary<Component, Queue<Component>> ObjectPools = new();
        private static readonly Dictionary<Component, GameObject> Containers = new();
        private static ObjectPoolRoot objectPoolRoot;

        public static void ReservePool<T>(T prefab, uint reservedAmount, bool useAutomaticReturn = true) where T : Component
        {
            CreatePoolIfNotExists(prefab);
            
            for (int i = 0; i < reservedAmount; i++)
            {
                CreatePooledObject(prefab, useAutomaticReturn);
            }
        }
        
        public static T GetFromPool<T>(T prefab, bool useAutomaticReturn = true, Transform parent = null) where T : Component
        {
            CreatePoolIfNotExists(prefab);
            
            if (ObjectPools[prefab].Count == 0)
            {
                CreatePooledObject(prefab, useAutomaticReturn);
            }

            Component component = ObjectPools[prefab].Dequeue();
            
            T pooledObject = (T) component;
            pooledObject.gameObject.SetActive(true);
            pooledObject.transform.SetParent(parent);
            
            return pooledObject;
        }

        public static void ReturnToPool<T>(T prefab, GameObject gameObject) where T : Component
        {
            CreatePoolIfNotExists(prefab);
            CreateContainerIfNotExists(prefab);

            T pooledObject = gameObject.GetComponent<T>();
            objectPoolRoot.SetParentDelayed(pooledObject.transform, Containers[prefab].transform);
            
            if (gameObject.GetComponent<AutomaticPoolReturner>() == null)
            {
                gameObject.SetActive(false);
            }
            
            ObjectPools[prefab].Enqueue(pooledObject);
        }

        public static void DestroyAllPooledObjects()
        {
            foreach (Component pooledObject in ObjectPools.Values.SelectMany(pooledObjects => pooledObjects))
            {
                if(pooledObject != null)
                {
                    Object.Destroy(pooledObject.gameObject);
                }
            }
            
            ObjectPools.Clear();
            OnPoolCleared();
        }

        private static void CreatePooledObject<T>(T prefab, bool useAutomaticReturn = true) where T : Component
        {
            CreateContainerIfNotExists(prefab);

            GameObject parent = Containers[prefab];
            T instance = Object.Instantiate(prefab, parent.transform);
            instance.gameObject.SetActive(false);

            if (useAutomaticReturn)
            {
                AutomaticPoolReturner automaticPoolReturner = instance.gameObject.AddComponent<AutomaticPoolReturner>();
                automaticPoolReturner.Initialize(prefab);
            }
            
            ObjectPools[prefab].Enqueue(instance);
        }
        
        private static void CreatePoolIfNotExists<T>(T prefab) where T : Component
        {
            if (!ObjectPools.ContainsKey(prefab))
            {
                ObjectPools[prefab] = new Queue<Component>();
            }
        }

        private static void CreateContainerIfNotExists<T>(T prefab) where T : Component
        {
            if (objectPoolRoot == null)
            {
                objectPoolRoot = CreateContainer("Object Pool").AddComponent<ObjectPoolRoot>();
            }

            if (!Containers.ContainsKey(prefab))
            {
                GameObject container = CreateContainer($"{prefab.name} Pool");
                container.transform.SetParent(objectPoolRoot.transform);
                Containers[prefab] = container;
            }
        }

        private static GameObject CreateContainer(string containerName)
        {
            GameObject container = new(containerName);
            
            container.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            container.transform.localScale = Vector3.one;

            return container;
        }
    }
}
