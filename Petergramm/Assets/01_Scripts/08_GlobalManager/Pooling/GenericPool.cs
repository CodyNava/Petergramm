using System.Collections.Generic;
using System.Linq;
using _01_Scripts._07_Enemy.Runtime;
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
         
         if (FreshPool.Count == 0)
         {
            T n = Object.Instantiate(obj, position, Quaternion.identity);
            GravePool.Add(n);
            n.gameObject.SetActive(true);
            return n;
         }

         var m = FreshPool[^1];
         GravePool.Add(m);
         FreshPool.RemoveAt(FreshPool.Count - 1);
         m.transform.position = position;
         m.gameObject.SetActive(true);
         return m;
         
         
      }
      
      public static void ReturnToPool(T obj)
      {
         obj.gameObject.SetActive(false);
         FreshPool.Add(obj);
         GravePool.Remove(obj);
      }
      

      public static void ClearPools()
      {
         FreshPool.Clear();
         GravePool.Clear();
      }

      //DEBUG
      public static int ReturnPoolItemsGraveDebug()
      {
         return GravePool.ToList().Count();
      }
      public static int ReturnPoolItemsFreshDebugs()
      {
         return FreshPool.ToList().Count();
      }
   }
}