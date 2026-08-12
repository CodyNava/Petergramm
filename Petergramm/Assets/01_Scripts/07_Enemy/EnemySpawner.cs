using System;
using _01_Scripts._01_Tower.Projectiles;
using _01_Scripts._01_Tower.Projectiles.UniqueProjectiles;
using _01_Scripts._07_Enemy.Data;
using _01_Scripts._07_Enemy.Runtime;
using _01_Scripts._08_GlobalManager.EnemyList;
using _01_Scripts._08_GlobalManager.GridToEnemyConnector;
using _01_Scripts._08_GlobalManager.Pooling;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _01_Scripts._07_Enemy
{
   public class EnemySpawner : MonoBehaviour
   {
      [SerializeField] private EnemySpawnDataSO esdso;

      
      //serialized for debugging reasons
      [SerializeField] private float _timer = 1f;
      [SerializeField] private bool _gameStarted = false;
      [SerializeField] private bool _gameEnded = false;
      [SerializeField] private int _spawnIndexOne = 0;
      [SerializeField] private int _spawnIndexTwo = 0;
      [SerializeField]private int _currentWave = 0;
      
      

      //DEBUG
      [Dropdown("_dropDownInts")]
      public int dropDownInt;
      private int[] _dropDownInts = new int[] { 0, 1 };

      
      
      private void Update()
      {
         if (!_gameStarted || _gameEnded) return;
         SpawnEnemiesByInterval();
      }

      public void StartGameButton() => _gameStarted = true;

      [Button]
      public void SpawnEnemyDebug()
      {
         var spawnPoint = esdso.spawnData.spawnPoint;
         var currentEnemy = esdso.spawnData.enemies[dropDownInt];
         var newEnemy = currentEnemy.Get();
         newEnemy.gameObject.transform.position = spawnPoint;
         newEnemy.gameObject.SetActive(true);
      }

      [Button]
      private void SpawnRandomEnemyDebug()
      {
         var spawnPoint = esdso.spawnData.spawnPoint + new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-7f, 7f));
         var currentEnemy = esdso.spawnData.enemies[Random.Range(0, esdso.spawnData.enemies.Count)];
         var newEnemy = currentEnemy.Get();
         newEnemy.gameObject.transform.position = spawnPoint;
         newEnemy.gameObject.SetActive(true);
      }

      [Button]
      public void ShowCurrentEnemiesDebug()
      {
         Debug.Log("CurrentEnemies IN LIST\n" + EnemyList.EnemyGameObjects.Count);
         //Debug.Log("CurrentENEMIES IN POOL\n" + GenericPool<EnemyRuntime>.ReturnPoolItemsGraveDebug());
         Debug.Log("CurrentPROJECTILES IN Fresh POOL\n" + GenericPool<BasketballProjectile>.ReturnPoolItemsFreshDebugs());
         Debug.Log("CurrentPROJECTILES IN Grave POOL\n" + GenericPool<BasketballProjectile>.ReturnPoolItemsGraveDebug());
         Debug.Log("CurrentPROJECTILES IN Fresh2 POOL\n" + GenericPool<ProjectileRuntime<BasketballProjectile>>.ReturnPoolItemsFreshDebugs());
         Debug.Log("CurrentPROJECTILES IN Grave2 POOL\n" + GenericPool<ProjectileRuntime<BasketballProjectile>>.ReturnPoolItemsGraveDebug());
         ;
      }
      //DEBUG

      public void SpawnRandomEnemyNormal() { SpawnRandomEnemyDebug(); }

      public void SpawnRandomEnemyTenTimes()
      {
         for (var i = 0; i < 10; i++) SpawnRandomEnemyDebug();
      }

      private bool Refresh(int currentWave)
      {
         var waveData = esdso.waveData;
         var totalEnemies = waveData[currentWave].enemyTypes[0].enemyCount +
                            waveData[currentWave].enemyTypes[1].enemyCount;
         var spawnInterval = waveData[currentWave].waveTime / totalEnemies;

         _timer += Time.deltaTime;

         if (_timer < spawnInterval) return false;
         _timer = 0f;
         return true;
      }

      private void SpawnEnemiesByInterval()
      {
         var waveData = esdso.waveData;
         if (!Refresh(_currentWave)) return;
         var spawnData = esdso.spawnData;
         
         var spawnAmountOne = waveData[_currentWave].enemyTypes[0].enemyCount;
         var spawnAmountTwo = waveData[_currentWave].enemyTypes[1].enemyCount;
         
         float coinFlip = Random.Range(spawnAmountOne, spawnAmountTwo);

         var runtimeOne = (spawnData.enemies[0], 0);
         var runtimeTwo = (spawnData.enemies[1], 1);

         var enemyExhaustedOne = _spawnIndexOne >= spawnAmountOne;
         var enemyExhaustedTwo = _spawnIndexTwo >= spawnAmountTwo;

         var enemyToSpawn = (coinFlip > spawnAmountOne && !enemyExhaustedTwo) || enemyExhaustedOne ? runtimeTwo : runtimeOne;

         SpawnEnemy(enemyToSpawn.Item1, enemyToSpawn.Item2);

         bool allEnemiesSpawned = _spawnIndexOne >= spawnAmountOne && _spawnIndexTwo >= spawnAmountTwo;

         if (!allEnemiesSpawned) return; //todo: and all enemies ded
         _currentWave++;
         GridToEnemyConnector.currentWave = _currentWave;
         _spawnIndexOne = 0;
         _spawnIndexTwo = 0;
         if (_currentWave >= waveData.Length) EndGame();
      }

      private void SpawnEnemy(EnemyRuntime enemyRuntime, int index)
      {
         
         if (!enemyRuntime)  return;
         switch (index)
         {
            case 0: _spawnIndexOne++; break;
            case 1: _spawnIndexTwo++; break;
         }

         Vector3 spawnPoint = esdso.spawnData.spawnPoint;
         EnemyRuntime newEnemy = enemyRuntime.Get();
         newEnemy.gameObject.transform.position = spawnPoint;
         newEnemy.gameObject.SetActive(true);
      }

      private void EndGame() => _gameEnded = true;
   }
}