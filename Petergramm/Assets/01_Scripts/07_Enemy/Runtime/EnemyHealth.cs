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

      public void TakeDamage(float damage)
      {
         this.currentHp -= damage;
         if (!(this.currentHp <= 0)) return;

         this.currentHp = 0;
         this.Die();
      }

      private void Die() => Destroy(this.gameObject);
   }
}