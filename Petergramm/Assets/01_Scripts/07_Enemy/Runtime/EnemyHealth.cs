using _01_Scripts._01_Tower.Data;
using _01_Scripts._01_Tower.Projectiles;
using _01_Scripts._08_GlobalManager.EnemyList;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Runtime
{
   public class EnemyHealth : MonoBehaviour
   {
      [SerializeField] private EnemyRuntime enemyRuntime;
      [SerializeField] private float currentHp, maxHp;
      [SerializeField] private Animator  animator;
      private void Start()
      { 
         RefreshHp();
         RefreshRunAnimationBasedOnSpeed();
      }

      private void OnValidate()
      {
         enemyRuntime = this.GetComponent<EnemyRuntime>();
      }

      private void RefreshHp()
      {
         if (!this.enemyRuntime) return;
         
         this.maxHp = enemyRuntime.CurrentStats.maxHp;
         this.currentHp = maxHp;
      }

      private void OnTriggerEnter(Collider other)
      {
         if (!other.CompareTag("Projectile")) return;
         var projectileData = other.gameObject.GetComponent<ProjectileRuntime>();
         CalculateDamage(projectileData.Damage, (TowerDamageType)projectileData.DamageType);
         enemyRuntime.ApplySlow(projectileData.SlowPercent);
         RefreshRunAnimationBasedOnSpeed();
      }

      private void CalculateDamage(short damage, TowerDamageType damageType)
      {
         var finalDamage = enemyRuntime.EnemyBase.damageRules.GetFinalDamage(
            damage,
            damageType,
            enemyRuntime.CurrentStats.armor
         );
         
         TakeDamage(finalDamage);
      }

      private void TakeDamage(float damage)
      {
         this.currentHp -= damage;
         if (!(this.currentHp <= 0)) return;

         this.currentHp = 0;
         this.Die();
      }
      private void Die()
      {
         EnemyList.RemoveEnemyFromList(this.gameObject);
         animator.SetTrigger("Died");
        // this.gameObject.SetActive(false);
      }
      
      private void RefreshRunAnimationBasedOnSpeed()
      {
            animator.SetFloat("Speed", enemyRuntime.CurrentStats.movement.moveSpeed / 5f);
      }
   }
}