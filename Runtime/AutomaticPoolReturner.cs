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
        
        private void OnDisable()
        {
            if (_prefab && this != null)
            {
                ObjectPool.ReturnToPool(_prefab, gameObject);
            }
        }
    }
}
