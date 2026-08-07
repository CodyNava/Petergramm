using _01_Scripts._01_Tower.Projectiles;
using _01_Scripts._07_Enemy.Data;
using _01_Scripts._07_Enemy.Runtime;
using _01_Scripts._08_GlobalManager.EnemyList;
using _01_Scripts._08_GlobalManager.Pooling;
using NaughtyAttributes;
using UnityEngine;

namespace _01_Scripts._07_Enemy
{
   public class EnemySpawner : MonoBehaviour
   {
      [SerializeField] private EnemySpawnDataSO esdso;
      
      //DEBUG
      [Dropdown("_dropDownInts")]
      public int dropDownInt;
      private int[] _dropDownInts = new int[] { 0, 1 };
      [Button]
      public void SpawnEnemyDebug()
      {
         var spawnPoint = esdso.spawnData.spawnPoint;
         var currentEnemy = esdso.spawnData.enemies[dropDownInt];
         var newEnemy = GenericPool<EnemyRuntime>.GetFromPool(currentEnemy);
         newEnemy.gameObject.transform.position = spawnPoint;
         newEnemy.gameObject.SetActive(true);
         
      }
      [Button]
      private void SpawnRandomEnemyDebug()
      {
         var spawnPoint = esdso.spawnData.spawnPoint + new Vector3(Random.Range(-10f, 10f), 0f ,Random.Range(-10f, 10f));
         var currentEnemy = esdso.spawnData.enemies[Random.Range(0, esdso.spawnData.enemies.Count)];
         var newEnemy = GenericPool<EnemyRuntime>.GetFromPool(currentEnemy);
         newEnemy.gameObject.transform.position = spawnPoint;
         newEnemy.gameObject.SetActive(true);
         
      }
      
      [Button]
      public void ShowCurrentEnemiesDebug()
      {
         Debug.Log("CurrentEnemies IN LIST\n" + EnemyList.EnemyGameObjects.Count);
         Debug.Log("CurrentENEMIES IN POOL\n" + GenericPool<EnemyRuntime>.ReturnPoolItemsDebug());
         Debug.Log("CurrentPROJECTILES IN POOL\n" + GenericPool<ProjectileRuntime>.ReturnPoolItemsDebug());;
         
      }
      //DEBUG

      public void SpawnRandomEnemyNormal()
      {
         SpawnRandomEnemyDebug();
      }

      public void SpawnRandomEnemyTenTimes()
      {
         for(var i = 0; i < 10; i++)
            SpawnRandomEnemyDebug();
      }
      
      private void SpawnEnemiesByInterval()
      {
         
      }
      
   }
}
