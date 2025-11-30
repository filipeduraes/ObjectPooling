using System;
using UnityEngine;

namespace IdeaToGame.ObjectPooling
{
    public class AutomaticPoolReturner : MonoBehaviour
    {
        private Component _prefab;

        public void Initialize<T>(T prefab) where T : Component
        {
            _prefab = prefab;
        }
        
        private void OnEnable()
        {
            ObjectPool.OnPoolCleared += DestroySelf;
        }

        private void OnDisable()
        {
            if (_prefab && this != null)
            {
                ObjectPool.ReturnToPool(_prefab, gameObject);
            }
            
            ObjectPool.OnPoolCleared -= DestroySelf;
        }
        
        private void DestroySelf()
        {
            if (this != null)
            {
                Destroy(gameObject);
            }
        }
    }
}
