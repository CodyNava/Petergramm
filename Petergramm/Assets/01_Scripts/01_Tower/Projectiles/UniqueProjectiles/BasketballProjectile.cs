using _01_Scripts._08_GlobalManager.Pooling;
using UnityEngine;

namespace _01_Scripts._01_Tower.Projectiles.UniqueProjectiles
{
   public class BasketballProjectile : ProjectileRuntime
   {
      private void Update()
      {
         Refresh();
         // if (!Target) ReturnToPool();
      }

      private void ReturnToPool()
      {
         this.gameObject.SetActive(false);
         GenericPool<BasketballProjectile>.ReturnToPool(this);
      }

      private void OnTriggerEnter(Collider other)
      {
         if (!other.CompareTag("Enemy")) return;
         ReturnToPool();
      }
   }
}