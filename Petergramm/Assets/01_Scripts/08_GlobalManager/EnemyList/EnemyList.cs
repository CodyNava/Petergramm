using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts._08_GlobalManager.EnemyList
{
   public static class EnemyList
   {
      public static readonly List<GameObject> EnemyGameObjects = new List<GameObject>();
      public static void AddEnemyToList(this GameObject enemy) => EnemyGameObjects.Add(enemy);
      public static void RemoveEnemyFromList(this GameObject enemy) => EnemyGameObjects.Remove(enemy);
      
   }
}