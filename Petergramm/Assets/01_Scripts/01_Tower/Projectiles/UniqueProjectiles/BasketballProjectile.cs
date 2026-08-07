using _01_Scripts._08_GlobalManager.Pooling;
using UnityEngine;

namespace _01_Scripts._01_Tower.Projectiles.UniqueProjectiles
{
   public class BasketballProjectile : ProjectileRuntime<BasketballProjectile>
   {
      private void FixedUpdate()
      {
         Refresh();
         Detection();
         // if (!Target) ReturnToPool();
      }
      
      private void Detection()
      {
         if (ValidateEnemies()) { ReturnToPool(); return; }
         if (!DetectCollisions()) return;
         CollisionCaller(this);
         if (FindTargetToBounce()) return;
         ReturnToPool();
      }
   }
}