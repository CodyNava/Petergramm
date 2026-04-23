using UnityEngine;

namespace _01_Scripts._01_Tower.RuntTime
{
   public class TowerHealth : MonoBehaviour
   {
      [SerializeField] private TowerRuntime towerRuntime;
      [SerializeField] private float currentHp, maxHp;

      private void OnValidate()=>
         towerRuntime = this.GetComponent<TowerRuntime>();
      private void Start() => this.RefreshHp(true);

      private void RefreshHp(bool fillCurrentHp)
      {
         if (!this.towerRuntime) return;

         this.maxHp = this.towerRuntime.CurrentStats.maxHp;
         this.currentHp = fillCurrentHp ? this.maxHp : Mathf.Min(this.currentHp, this.maxHp);
      }

      public void TakeDamage(float damage)
      {
         //todo add taking damage if we decide to make towers killable or not

         this.currentHp -= damage;
         if (!(this.currentHp <= 0)) return;

         this.currentHp = 0;
         this.Die();
      }

      private void Die() => Destroy(this.gameObject);
   }
}