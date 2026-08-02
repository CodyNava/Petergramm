using _01_Scripts._01_Tower.Data;
using _01_Scripts._01_Tower.Projectiles;
using _01_Scripts._08_GlobalManager.EnemyList;
using _01_Scripts._08_GlobalManager.Pooling;
using NaughtyAttributes;
using Unity.Profiling;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Runtime
{
   public class EnemyHealth : MonoBehaviour
   {
      private static readonly int Died = Animator.StringToHash("Died");
      private static readonly int Speed = Animator.StringToHash("Speed");
      [SerializeField] private EnemyRuntime enemyRuntime;
      [SerializeField] private float currentHp, maxHp;
      [SerializeField] private Animator animator;

      private bool _hasDied = false;

      private static readonly ProfilerMarker EnemyHealthMarkerTrigger = new ProfilerMarker("EnemyHealthOnTriggerEnter");
      private static readonly ProfilerMarker EnemyHealthCalculateDamage = new ProfilerMarker("EnemyHealthCalculateDamage");
      private static readonly ProfilerMarker EnemyHealthDie = new ProfilerMarker("EnemyHealthDie");

      private void OnEnable()
      {
         RefreshHp();
         RefreshRunAnimationBasedOnSpeed();
         _hasDied = false;
      }

      private void OnValidate() { enemyRuntime = this.GetComponent<EnemyRuntime>(); }

      private void RefreshHp()
      {
         if (!this.enemyRuntime) return;

         this.maxHp = enemyRuntime.CurrentStats.maxHp;
         this.currentHp = maxHp;
      }

      private void OnTriggerEnter(Collider other)
      {
         using (EnemyHealthMarkerTrigger.Auto())
         {
            if (!other.CompareTag("Projectile")) return;
            var projectileData = other.gameObject.GetComponent<ProjectileRuntime>();
            CalculateDamage(projectileData.Damage, (TowerDamageType)projectileData.DamageType);

            if (projectileData.SlowPercent == 0 || _hasDied) return;
            enemyRuntime.ApplySlow(projectileData.SlowPercent);
            RefreshRunAnimationBasedOnSpeed();
         }
      }

      private void CalculateDamage(short damage, TowerDamageType damageType)
      {
         using (EnemyHealthCalculateDamage.Auto())
         {
            float finalDamage = enemyRuntime.EnemyBase.damageRules.GetFinalDamage(
               damage,
               damageType,
               enemyRuntime.CurrentStats.armor
            );

            TakeDamage(finalDamage);
         }
      }

      private void TakeDamage(float damage)
      {
         using (EnemyHealthDie.Auto())
         {
            this.currentHp -= damage;
            if (!(this.currentHp <= 0) || _hasDied) return;

            this.currentHp = 0;
            this.Die();
         }
      }

      [Button]
      private void Die()
      {
         enemyRuntime.ApplySlow(99); //stop target when ddeeed
         _hasDied = true;
         EnemyList.RemoveEnemyFromList(this.gameObject);
         animator.SetTrigger(Died);
         enemyRuntime.ReturnToPoolOnDeath();
      }

      private void RefreshRunAnimationBasedOnSpeed() =>
         animator.SetFloat(Speed, enemyRuntime.CurrentStats.movement.moveSpeed / 1.5f);
   }
}