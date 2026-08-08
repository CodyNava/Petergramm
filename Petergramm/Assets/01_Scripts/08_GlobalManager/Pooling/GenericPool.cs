using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _01_Scripts._08_GlobalManager.Pooling
{
   public static class GenericPool<T> where T : MonoBehaviour
   {
      private static readonly List<T> FreshPool = new();
      private static readonly HashSet<T> GravePool = new();

      public static T GetFromPool(T obj)
      {
         if (FreshPool.Count == 0)
         {
            var n = Object.Instantiate(obj);
            GravePool.Add(n);
            return n;
         }

         var m = FreshPool[^1];
         GravePool.Add(m);
         FreshPool.RemoveAt(FreshPool.Count - 1);

         return m;
      }

      public static T GetFromPool(T obj, Vector3 position)
      {
         var instance = GetFromPool(obj);
         instance.transform.position = position;
         instance.gameObject.SetActive(true);
         return instance;
      }

      public static void ReturnToPool(T obj)
      {
         obj.gameObject.SetActive(false);
         FreshPool.Add(obj);
         GravePool.Remove(obj);
      }

      //DEBUG
      public static int ReturnPoolItemsDebug()
      {
         var count = 0;
         foreach (var item in FreshPool.ToList())
         {
           
            
         }

         foreach (var item in GravePool.ToList())
         {
           count++;
         }

         return count;
      }
   }
}