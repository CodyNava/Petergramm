using System;
using System.Collections;
using _01_Scripts._01_Tower.Data;
using _01_Scripts._01_Tower.Projectiles;
using _01_Scripts._08_GlobalManager.EnemyList;
using _01_Scripts._08_GlobalManager.GridToEnemyConnector;
using Unity.Profiling;
using UnityEngine;
using Random = UnityEngine.Random;

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

      private int _id;
      public int ID => _id;

      private static readonly ProfilerMarker EnemyHealthMarkerTrigger = new ProfilerMarker("EnemyHealthOnTriggerEnter");
      private static readonly ProfilerMarker EnemyHealthCalculateDamage =
         new ProfilerMarker("EnemyHealthCalculateDamage");
      private static readonly ProfilerMarker EnemyHealthDie = new ProfilerMarker("EnemyHealthDie");

      private void Start()
      {
         RefreshHp();
         _id = Random.Range(int.MinValue, int.MaxValue);
         _hasDied = false;
         RefreshRunAnimationBasedOnSpeed();
      }

      private void OnEnable()
      {
         StartCoroutine(RefreshOnEnableWithDelay());
      }

      private IEnumerator RefreshOnEnableWithDelay()
      {
         yield return new WaitForSeconds(0.05f);
         RefreshHp();
         _id = Random.Range(int.MinValue, int.MaxValue);
         _hasDied = false;
         RefreshRunAnimationBasedOnSpeed();
      }
      

      private void RefreshHp()
      {
         if (!this.enemyRuntime) return;

         this.maxHp = enemyRuntime.CurrentStats.maxHp;
         this.currentHp = maxHp;
      }

      public void Collision(ProjectileRuntime proj)
      {
         using ProfilerMarker.AutoScope _ = EnemyHealthMarkerTrigger.Auto();

         CalculateDamage(proj.Damage, (TowerDamageType)proj.DamageType);

         if (proj.SlowPercentage == 0 || _hasDied) return;
         enemyRuntime.ApplySlow(proj.SlowPercentage);
         RefreshRunAnimationBasedOnSpeed();
      }

      private void CalculateDamage(short damage, TowerDamageType damageType)
      {
         using ProfilerMarker.AutoScope _ = EnemyHealthCalculateDamage.Auto();
         
         float finalDamage = enemyRuntime.EnemyBase.damageRules.GetFinalDamage(
            damage,
            damageType,
            enemyRuntime.CurrentStats.armor
         );

         TakeDamage(finalDamage);
      }

      private void TakeDamage(float damage)
      {
         using var _ = EnemyHealthDie.Auto();
         this.currentHp -= damage;
         if (!(this.currentHp <= 0) || _hasDied) return;

         this.currentHp = 0;
         this.Die();
      }
      
      private void Die()
      {
         enemyRuntime.ApplySlow(99); //stop target when ddeeed
         _hasDied = true;
         gameObject.RemoveEnemyFromList();
         animator.SetTrigger(Died);
         transform.position = new Vector3(-90f, 0f, 0f);
         GridToEnemyConnector.EnemyDroppedGold(enemyRuntime.CurrentStats.goldDrop);
         enemyRuntime.Dead = true;
      }

     

      private void RefreshRunAnimationBasedOnSpeed() =>
         animator.SetFloat(Speed, enemyRuntime.CurrentStats.movement.moveSpeed / 1.5f);
      
   }
}