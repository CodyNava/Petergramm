using _01_Scripts._08_GlobalManager.Pooling;
using UnityEngine;

namespace _01_Scripts._01_Tower.Projectiles.UniqueProjectiles
{
   public class BaseballProjectile : ProjectileRuntime<BaseballProjectile>
   {
      private void FixedUpdate()
      {
         Refresh();
         Detection();
         // if (!Target) ReturnToPool();
      }

      private void Detection()
      {
         if (!DetectCollisions()) return;
         if (FindTargetToBounce()) return;
         ReturnToPool();
      }
   }
}