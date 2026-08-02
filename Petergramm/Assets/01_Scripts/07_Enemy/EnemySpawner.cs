using System.Collections.Generic;
using _01_Scripts._07_Enemy.Data;
using _01_Scripts._08_GlobalManager.EnemyList;
using NaughtyAttributes;
using UnityEngine;

namespace _01_Scripts._07_Enemy
{
   public class EnemySpawner : MonoBehaviour
   {
      [SerializeField] private EnemySpawnDataSO esdso;

      [Dropdown("_dropDownInts")]
      public int dropDownInt;
      
      private int[] _dropDownInts = new int[] { 0, 1 };
      [Button]
      public void SpawnEnemy()
      {
         var spawnPoint = esdso.spawnData.spawnPoint;
         var currentEnemy = esdso.spawnData.enemies[dropDownInt];
         Instantiate(currentEnemy, spawnPoint, Quaternion.identity);
         EnemyList.Enemies.Add(currentEnemy);
      }

      [Button]
      public void DespawnEnemies()
      {
         EnemyList.Enemies.Clear();
      }
      
      
   }
}
