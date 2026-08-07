using System.Collections.Generic;
using _01_Scripts._07_Enemy.Runtime;
using _01_Scripts._08_GlobalManager.Pooling;
using UnityEngine;

namespace _01_Scripts._08_GlobalManager.EnemyList
{
   public static class EnemyList
   {
      public static readonly List<GameObject> EnemyGameObjects = new List<GameObject>();
      public static readonly List<EnemyHealth> EnemyHealths = new List<EnemyHealth>();

      public static void AddEnemyToList(GameObject enemy) => EnemyGameObjects.Add(enemy);

      public static void AddEnemyToList(EnemyHealth enemy) => EnemyHealths.Add(enemy);

      public static void RemoveEnemyFromList(GameObject enemy) => EnemyGameObjects.Remove(enemy);

      public static void RemoveEnemyFromList(EnemyHealth enemy) =>
         EnemyHealths.Remove(enemy);
   }
}