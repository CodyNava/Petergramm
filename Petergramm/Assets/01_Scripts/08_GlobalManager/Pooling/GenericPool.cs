using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts._08_GlobalManager.Pooling
{
    public static class GenericPool<T> where T : MonoBehaviour
    {
        private static readonly List<T> FreshPool = new();
        private static readonly HashSet<T> GravePool = new();
        
        public static T GetFromPool(GameObject obj)
        {
            if (FreshPool.Count == 0)
            {
                var n = Object.Instantiate(obj).GetComponent<T>() ;
                 GravePool.Add(n);
                 FreshPool.Remove(n);
                 return n;
            }
            
            var m = FreshPool[^1];

            GravePool.Add(m);
            FreshPool.Remove(m);

            return m;
        }

        public static void ReturnToPool(T obj)
        {
            FreshPool.Add(obj);
            GravePool.Remove(obj);
        }

        public static int ReturnPoolCount()
        {
            return FreshPool.Count + GravePool.Count;
        }
    }
}