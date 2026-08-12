using System;
using _01_Scripts._07_Enemy.Data;
using _01_Scripts._08_GlobalManager.EnemyList;
using _01_Scripts._08_GlobalManager.GridToEnemyConnector;
using _01_Scripts._08_GlobalManager.Pooling;
using Unity.Profiling;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Runtime
{
   public abstract class EnemyRuntime : MonoBehaviour
   {
      [SerializeField] private EnemyBaseSO enemyBase;
      [SerializeField] private EnemyStats currentStats;

      private bool _slowed;
      private int _lastSlowValue;
      private float _slowDuration = 1f;

      public bool Dead = false;

      private static readonly ProfilerMarker EnemyRuntimeApplyStats = new ProfilerMarker("EnemyRuntimeApplyStats");
      private static readonly ProfilerMarker EnemyRuntimeUpdate = new ProfilerMarker("EnemyRuntimeUpdate");
      private static readonly ProfilerMarker EnemyRuntimeRefreshSlow = new ProfilerMarker("EnemyRuntimeRefreshSlow");

      //Getter
      public EnemyStats CurrentStats => currentStats;
      public EnemyBaseSO EnemyBase { get => enemyBase; set => enemyBase = value; }

      protected void Initialize()
      {
         ApplyStats();
      }

      protected void OnReenter()
      {
         Dead = false;
         ApplyStats();
         gameObject.AddEnemyToList();
      }

      private void ApplyStats()
      {
         using (EnemyRuntimeApplyStats.Auto())
         {
            currentStats.maxHp = enemyBase.stats.maxHp *
                                 enemyBase.waveData.waveData[GridToEnemyConnector.currentWave].hpMult;
            currentStats.armor = enemyBase.stats.armor;
            currentStats.damage = enemyBase.stats.damage;
            currentStats.attacksPerSecond = enemyBase.stats.attacksPerSecond;
            currentStats.movement = enemyBase.stats.movement;
            currentStats.range = enemyBase.stats.range;
            currentStats.goldDrop = enemyBase.stats.goldDrop *
                                    enemyBase.waveData.waveData[GridToEnemyConnector.currentWave].moneyMult;
         }
      }

      //todo debuffs in eine eigene class stecken und dort managen
      public void ApplySlow(byte slow)
      {
         if (slow <= 0) return;

         _slowed = true;
         float baseMovementSpeed = enemyBase.stats.movement.moveSpeed;
         float moveSpeedSlowDif = (baseMovementSpeed * RefreshSlow(slow) / 100f);
         this.currentStats.movement.moveSpeed = baseMovementSpeed - moveSpeedSlowDif;
         _lastSlowValue = slow;
      }

      protected void ResetSlow()
      {
         if (!_slowed) return;
         const float duration = 1f;
         _slowDuration -= Time.deltaTime;

         if (_slowDuration >= 0f) return;
         this.currentStats.movement.moveSpeed = enemyBase.stats.movement.moveSpeed;
         _slowed = false;
         _slowDuration = duration;
      }

      private int RefreshSlow(byte slow)
      {
         using (EnemyRuntimeRefreshSlow.Auto()) { return _lastSlowValue >= slow ? _lastSlowValue : slow; }
      }
      //todo


      
      
      protected bool CheckIfDead()
      {
         return Dead;
      }
      
      
      public abstract EnemyRuntime Get();
      public abstract EnemyRuntime Get(Vector3 position);
      
      
   }

   
   public class EnemyRuntime<T> : EnemyRuntime where T : EnemyRuntime<T>
   {
      
      public override EnemyRuntime Get() { return GenericPool<EnemyRuntime<T>>.GetFromPool(this); }

      public override EnemyRuntime Get(Vector3 position)
      {
         return GenericPool<EnemyRuntime<T>>.GetFromPool(this, position);
      }
      
      protected void ReturnToPool()
      {
         this.gameObject.SetActive(false);
         GenericPool<EnemyRuntime<T>>.ReturnToPool(this);
      }
   }
   
}