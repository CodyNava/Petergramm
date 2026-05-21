using System;
using _01_Scripts._01_Tower.Data;
using _01_Scripts._01_Tower.Projectiles;
using _01_Scripts._01_Tower.RuntTime;
using _01_Scripts._07_Enemy.Data;
using _01_Scripts._08_GlobalManager.DamageRules;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Runtime
{
   public class EnemyHealth : MonoBehaviour
   {
      [SerializeField] private EnemyRuntime enemyRuntime;
      [SerializeField] private float currentHp, maxHp;
      private void Start() => RefreshHp();

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
         projectileData.ReturnToPool();
         other.gameObject.SetActive(false);
      }

      private void CalculateDamage(short damage, TowerDamageType damageType)
      {
         var finalDamage = enemyRuntime.EnemyBase.damageRules.GetFinalDamage(
            damage,
            damageType,
            enemyRuntime.CurrentStats.armor.armorType,
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

      

      private void Die() => Destroy(this.gameObject);
   }
}