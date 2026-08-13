using System;
using _01_Scripts._01_Tower.Placement;
using _01_Scripts._01_Tower.Projectiles;
using _01_Scripts._01_Tower.Projectiles.UniqueProjectiles;
using _01_Scripts._07_Enemy.Runtime;
using _01_Scripts._07_Enemy.Runtime.UniqueEnemies;
using _01_Scripts._08_GlobalManager.EnemyList;
using _01_Scripts._08_GlobalManager.Pooling;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace _01_Scripts._09_Debugging
{
   public class DebugButtons : MonoBehaviour
   {
      [SerializeField] private TextMeshProUGUI enemyCounter;
      [SerializeField] private TextMeshProUGUI projCounter;
      [SerializeField] private Light followMouseLight;
      [SerializeField] private TowerPlacement towerPlacement;

      private void FixedUpdate()
      {
         RefreshEnemyCounter();
         RefreshProjectileCounter();
      }

      private void RefreshEnemyCounter()
      {
         enemyCounter.text = $"Enemy\nCount\n{GenericPool<EnemyRuntime>.ReturnPoolItemsGraveDebug()}";
      }
      
      private void RefreshProjectileCounter()
      {
         projCounter.text = $"Proj\nCount\n{GenericPool<ProjectileRuntime<BasketballProjectile>>.ReturnPoolItemsGraveDebug()}";
      }

      public void TestLightFollowMouse(Vector3 position)
      {
         followMouseLight.transform.position = position + Vector3.up * 5f;
      }

      private void Start()
      {
         GenericPool<EnemyRuntime<FatZombie>>.ClearPools();
         GenericPool<EnemyRuntime<NormalZombie>>.ClearPools();
         GenericPool<ProjectileRuntime<BasketballProjectile>>.ClearPools();
         GenericPool<ProjectileRuntime<FootballProjectile>>.ClearPools();
         GenericPool<ProjectileRuntime<BaseballProjectile>>.ClearPools();
         GenericPool<ProjectileRuntime<GolfballProjectile>>.ClearPools();
         GenericPool<ProjectileRuntime<SoccerballProjectile>>.ClearPools();
         GenericPool<ProjectileRuntime<TennisballProjectile>>.ClearPools();
         EnemyList.ClearList();
      }
   }
}