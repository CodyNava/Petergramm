using _01_Scripts._07_Enemy.Data;
using _01_Scripts._08_GlobalManager.EnemyList;
using _01_Scripts._08_GlobalManager.GridToEnemyConnector;
using _01_Scripts._08_GlobalManager.Pooling;
using Unity.Profiling;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Runtime
{
   public class EnemyRuntime : MonoBehaviour
   {
      [SerializeField] private EnemyBaseSO enemyBase;
      [SerializeField] private EnemyStats currentStats;

      private bool _slowed;
      private int _lastSlowValue;
      private float _slowDuration = 1f;

      private static readonly ProfilerMarker EnemyRuntimeApplyStats = new ProfilerMarker("EnemyRuntimeApplyStats");
      private static readonly ProfilerMarker EnemyRuntimeUpdate = new ProfilerMarker("EnemyRuntimeUpdate");
      private static readonly ProfilerMarker EnemyRuntimeRefreshSlow = new ProfilerMarker("EnemyRuntimeRefreshSlow");

      //Getter
      public EnemyStats CurrentStats => currentStats;
      public EnemyBaseSO EnemyBase { get => enemyBase; set => enemyBase = value; }

      private void Awake() { ApplyStats(); }

      private void OnEnable()
      {
         ApplyStats();
         gameObject.AddEnemyToList();
      }

      private void Update()
      {
         using (EnemyRuntimeUpdate.Auto())
         {
            ResetSlow();
            
         }
      }

      private void ApplyStats()
      {
         using (EnemyRuntimeApplyStats.Auto())
         {
            currentStats.maxHp = enemyBase.stats.maxHp;
            currentStats.armor = enemyBase.stats.armor;
            currentStats.damage = enemyBase.stats.damage;
            currentStats.attacksPerSecond = enemyBase.stats.attacksPerSecond;
            currentStats.movement = enemyBase.stats.movement;
            currentStats.range = enemyBase.stats.range;
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

      private void ResetSlow()
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
         using (EnemyRuntimeRefreshSlow.Auto())
         {
           return _lastSlowValue >= slow ? _lastSlowValue : slow;
         }
      }
      //todo

      public void ReturnToPoolOnDeath() => GenericPool<EnemyRuntime>.ReturnToPool(this);


     
   }
}