using System;
using _01_Scripts._01_Tower.Data;
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
      private void Start() => RefreshHp(true);

      private void RefreshHp(bool fillCurrentHp)
      {
         if (!this.enemyRuntime) return;

         this.maxHp = enemyRuntime.CurrentStats.maxHp;
         this.currentHp = fillCurrentHp ? this.maxHp : Mathf.Min(this.currentHp, this.maxHp);
      }

      private void OnCollisionEnter(Collision other)
      {
         var projectileData = other.gameObject.GetComponent<ProjectileRuntime>();
         CalculateDamage(projectileData.Damage, (TowerDamageType)projectileData.DamageType);
      }

      private void CalculateDamage(short damage, TowerDamageType damageType)
      {
         var finalDamage = enemyRuntime.EnemyBase.damageRules.GetFinalDamage(
            damage,
            damageType,
            enemyRuntime.CurrentStats.armor.armorType
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