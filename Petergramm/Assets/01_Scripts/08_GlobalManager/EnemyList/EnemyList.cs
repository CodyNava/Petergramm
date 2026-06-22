using System.Collections.Generic;
using _01_Scripts._07_Enemy.Runtime;
using _01_Scripts._08_GlobalManager.Pooling;
using UnityEngine;

namespace _01_Scripts._08_GlobalManager.EnemyList
{
    public static class EnemyList
    {
        public static readonly List<GameObject> Enemies = new List<GameObject>();

        public static void AddEnemyToList(GameObject enemy)
            => Enemies.Add(enemy);
        public static void RemoveEnemyFromList(GameObject enemy)
            => Enemies.Remove(enemy);

        
    }
}