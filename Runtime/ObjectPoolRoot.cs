using System.Collections;
using UnityEngine;

namespace IdeaToGame.ObjectPooling
{
    public class ObjectPoolRoot : MonoBehaviour
    {
        public void SetParentDelayed(Transform child, Transform parent)
        {
            StartCoroutine(SetParentOnNextFrame(child, parent));
        }

        private IEnumerator SetParentOnNextFrame(Transform child, Transform parent)
        {
            yield return null;
            child.SetParent(parent);
        }
    }
}