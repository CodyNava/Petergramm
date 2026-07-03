using _01_Scripts._08_GlobalManager.Pooling;

namespace _01_Scripts._01_Tower.Projectiles.UniqueProjectiles
{
   public class TennisballProjectile : ProjectileRuntime
   {
      private void FixedUpdate()
      {
         Refresh();
         Detection();
         // if (!Target) ReturnToPool();
      }

      private void ReturnToPool()
      {
         this.gameObject.SetActive(false);
         GenericPool<TennisballProjectile>.ReturnToPool(this);
      }

      private void Detection()
      {
         if (!DetectCollisions()) return;
         if (FindTargetToBounce()) return;
         ReturnToPool();
      }
   }
}